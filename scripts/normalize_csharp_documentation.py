#!/usr/bin/env python3
"""Adds concise Persian XML summaries and safe regions to handwritten C# files."""

from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
PROJECTS = ("VisitorManagment.Core", "VisitorManagment.DataLayer", "VisitorManagment.Web")

METHOD_PATTERN = re.compile(
    r"^(?P<indent>\s*)(?:public|private|protected|internal)\s+"
    r"(?:(?:static|async|virtual|override|sealed|new|partial|extern)\s+)*"
    r"(?:[\w<>,.?\[\]\s]+)\s+(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*\("
)
TYPE_PATTERN = re.compile(r"\b(?:class|interface|struct)\s+[A-Za-z_][A-Za-z0-9_]*")


def describe(name: str) -> str:
    """Creates a short readable description from a conventional method name."""
    exact = {
        "OnGet": "اطلاعات موردنیاز صفحه را بارگذاری می‌کند.",
        "OnPost": "اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.",
        "Commit": "تغییرات جاری را در پایگاه داده ذخیره می‌کند.",
        "Dispose": "منابع استفاده‌شده را آزاد می‌کند.",
    }
    if name in exact:
        return exact[name]

    rules = (
        (("OnGet",), "درخواست دریافت اطلاعات صفحه را پردازش می‌کند."),
        (("OnPost",), "درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند."),
        (("Get", "Find", "Load", "Fetch", "Read"), "اطلاعات موردنیاز را دریافت می‌کند."),
        (("Add", "Create", "Insert", "Register", "Reg", "Save"), "اطلاعات جدید را اعتبارسنجی و ثبت می‌کند."),
        (("Edit", "Update", "Change", "Set"), "اطلاعات موجود را بررسی و به‌روزرسانی می‌کند."),
        (("Delete", "Remove"), "اطلاعات مشخص‌شده را حذف می‌کند."),
        (("Send", "Post", "Publish"), "اطلاعات را به مقصد موردنظر ارسال می‌کند."),
        (("Check", "Validate", "Is", "Has", "Can", "Exist"), "شرایط موردنظر را بررسی می‌کند."),
        (("Convert", "To", "Encode", "Decode"), "مقدار ورودی را به قالب موردنظر تبدیل می‌کند."),
        (("Build", "Generate", "Create"), "خروجی موردنیاز را تولید می‌کند."),
        (("Show", "Display"), "اطلاعات موردنیاز برای نمایش را آماده می‌کند."),
    )
    for prefixes, description in rules:
        if name.startswith(prefixes):
            return description
    return "عملیات مربوط به این بخش را انجام می‌دهد."


def has_summary(lines: list[str], index: int) -> bool:
    """Checks comments and attributes immediately preceding a declaration."""
    cursor = index - 1
    while cursor >= 0 and (not lines[cursor].strip() or lines[cursor].lstrip().startswith("[")):
        cursor -= 1
    return cursor >= 0 and lines[cursor].lstrip().startswith("///")


def method_indexes(lines: list[str]) -> list[tuple[int, re.Match[str]]]:
    """Returns method declarations while excluding constructors and control statements."""
    result = []
    for index, line in enumerate(lines):
        match = METHOD_PATTERN.match(line)
        if not match:
            continue
        name = match.group("name")
        if name in {"if", "for", "foreach", "while", "switch", "catch", "using", "lock"}:
            continue
        result.append((index, match))
    return result


def add_summaries(text: str) -> tuple[str, int]:
    """Adds missing XML summaries without changing method bodies or signatures."""
    lines = text.splitlines(keepends=True)
    additions = 0
    for index, match in reversed(method_indexes(lines)):
        if has_summary(lines, index):
            continue
        indent = match.group("indent")
        newline = "\r\n" if lines[index].endswith("\r\n") else "\n"
        summary = [
            f"{indent}/// <summary>{newline}",
            f"{indent}/// {describe(match.group('name'))}{newline}",
            f"{indent}/// </summary>{newline}",
        ]
        lines[index:index] = summary
        additions += 1
    return "".join(lines), additions


def add_safe_region(text: str) -> tuple[str, bool]:
    """Wraps members of simple, unsectioned files in one balanced region."""
    if "#region" in text or len(TYPE_PATTERN.findall(text)) != 1:
        return text, False

    lines = text.splitlines(keepends=True)
    methods = method_indexes(lines)
    if len(methods) < 2:
        return text, False

    first = methods[0][0]
    region_start = first
    while region_start > 0 and (not lines[region_start - 1].strip() or lines[region_start - 1].lstrip().startswith("///")):
        region_start -= 1
    newline = "\r\n" if lines[first].endswith("\r\n") else "\n"
    indent = methods[0][1].group("indent")

    depth = 0
    type_depth = None
    end_index = None
    for index, line in enumerate(lines):
        if type_depth is None and TYPE_PATTERN.search(line):
            type_depth = depth
        opens = line.count("{")
        closes = line.count("}")
        depth += opens - closes
        if type_depth is not None and index > first and depth == type_depth:
            end_index = index
            break

    if end_index is None or end_index <= first:
        return text, False

    lines.insert(end_index, f"{indent}#endregion{newline}")
    lines.insert(region_start, f"{indent}#region اعضا و متدهای کلاس{newline}{newline}")
    return "".join(lines), True


def repair_region_position(text: str) -> str:
    """Keeps region directives before XML documentation so docs bind to methods."""
    lines = text.splitlines(keepends=True)
    marker = "#region اعضا و متدهای کلاس"
    for index in range(len(lines) - 1, -1, -1):
        if marker not in lines[index]:
            continue
        cursor = index - 1
        while cursor >= 0 and (not lines[cursor].strip() or lines[cursor].lstrip().startswith("///")):
            cursor -= 1
        if cursor == index - 1:
            continue
        region = lines.pop(index)
        if index - 1 >= 0 and not lines[index - 1].strip():
            lines.pop(index - 1)
        lines.insert(cursor + 1, region)
        lines.insert(cursor + 2, "\r\n" if region.endswith("\r\n") else "\n")
    return "".join(lines)


def eligible(path: Path) -> bool:
    parts = path.parts
    return (
        path.suffix == ".cs"
        and "Migrations" not in parts
        and "obj" not in parts
        and not path.name.endswith(".Designer.cs")
        and path.name != "VisitorManagmentContextModelSnapshot.cs"
    )


def read_source(path: Path) -> tuple[str, str]:
    """Reads legacy source files while preserving their original encoding."""
    data = path.read_bytes()
    if data.startswith(b"\xff\xfe"):
        return data.decode("utf-16"), "utf-16"
    if data.startswith(b"\xfe\xff"):
        return data.decode("utf-16"), "utf-16"
    if data.startswith(b"\xef\xbb\xbf"):
        return data.decode("utf-8-sig"), "utf-8-sig"
    return data.decode("utf-8"), "utf-8"


def main() -> None:
    files_changed = summaries_added = regions_added = 0
    for project in PROJECTS:
        for path in (ROOT / project).rglob("*.cs"):
            if not eligible(path):
                continue
            disk_original, encoding = read_source(path)
            original = repair_region_position(disk_original)
            documented, count = add_summaries(original)
            sectioned, region_added = add_safe_region(documented)
            if sectioned == disk_original:
                continue
            path.write_text(sectioned, encoding=encoding)
            files_changed += 1
            summaries_added += count
            regions_added += int(region_added)
    print(f"files={files_changed} summaries={summaries_added} regions={regions_added}")


if __name__ == "__main__":
    main()
