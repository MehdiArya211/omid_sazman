(function ($) {
    "use strict";

    if (!$ || !$.fn || !$.fn.DataTable) { return; }

    var tableSequence = 0;
    var activeExport = null;
    var modalId = "gridExportColumnsModal";
    var persianLanguage = {
        emptyTable: "اطلاعاتی برای نمایش وجود ندارد",
        info: "نمایش _START_ تا _END_ از _TOTAL_ رکورد",
        infoEmpty: "هیچ رکوردی موجود نیست",
        infoFiltered: "(فیلترشده از _MAX_ رکورد)",
        lengthMenu: "نمایش _MENU_ رکورد",
        loadingRecords: "در حال بارگذاری...",
        processing: "در حال پردازش...",
        search: "جست‌وجو:",
        searchPlaceholder: "عبارت موردنظر...",
        zeroRecords: "رکوردی مطابق جست‌وجو پیدا نشد",
        paginate: { first: "اول", last: "آخر", next: "بعدی", previous: "قبلی" }
    };

    function cleanText(value) {
        return $("<div>").html(value == null ? "" : value.toString()).text().replace(/\s+/g, " ").trim();
    }

    function isReportPage() {
        return window.location.pathname.toLowerCase().indexOf("/admin/reports/") !== -1;
    }

    function shouldEnhance(table) {
        if (!table || table.dataset.datatableInitialized === "true") { return false; }
        if (table.matches(".no-datatable, .tableChild, #roomTable, #tblRooms, #fileTable")) { return false; }
        if (!table.tHead || !table.tBodies.length || !table.tHead.rows.length) { return false; }
        if (table.closest(".swal-modal")) { return false; }
        return table.hasAttribute("data-datatable") || table.id === "dataTable" || table.id === "myTable" ||
            table.id === "example1" || (isReportPage() && table.classList.contains("table"));
    }

    function ensureUniqueId(table) {
        var currentId = table.id;
        var duplicateCount = currentId ? Array.prototype.filter.call(document.querySelectorAll("[id]"), function (element) { return element.id === currentId; }).length : 0;
        if (!currentId || duplicateCount > 1) {
            table.id = "unifiedDataTable_" + (++tableSequence);
        }
    }

    function exportableColumns(api) {
        var columns = [];
        api.columns().every(function (index) {
            var header = this.header();
            var title = cleanText(header ? header.innerHTML : "");
            var blockedTitle = /^(عملیات|انتخاب|انتخاب همه)$/i.test(title);
            var blockedMarkup = header && (header.hasAttribute("data-no-export") || header.classList.contains("no-export"));
            if (title && !blockedTitle && !blockedMarkup) {
                columns.push({ index: index, title: title });
            }
        });
        return columns;
    }

    function createToolbar(api) {
        var container = $(api.table().container());
        if (container.find(".dataTables-toolbar").length) { return; }
        container.addClass("unified-datatable");
        container.closest(".table-responsive, #print_this").addClass("datatable-host");
        $(api.table().node()).removeClass("overflow-auto");

        var lengthSelect = container.find(".dataTables_length select").first();
        if (lengthSelect.length && !lengthSelect.find('option[value="-1"]').length) {
            lengthSelect.append('<option value="-1">همه</option>');
        }
        var filter = container.find(".dataTables_filter").first();
        var length = container.find(".dataTables_length").first();
        var toolbar = $('<div class="dataTables-toolbar"><div class="dataTables-toolbar__controls"></div><div class="dataTables-toolbar__exports"></div></div>');
        var controls = toolbar.find(".dataTables-toolbar__controls");
        var exports = toolbar.find(".dataTables-toolbar__exports");

        if (filter.length) {
            var searchInput = filter.find("input").first();
            searchInput.attr({ placeholder: "جست‌وجو در جدول...", "aria-label": "جست‌وجو در جدول" });
            filter.empty().append($('<label class="dataTables-search-box"><i class="ti-search" aria-hidden="true"></i><span class="sr-only">جست‌وجو</span></label>').append(searchInput));
            controls.append(filter);
        }
        if (length.length) {
            var select = length.find("select").first();
            select.attr("aria-label", "تعداد رکورد در هر صفحه");
            length.empty().append($('<label class="dataTables-length-box"><span>نمایش</span></label>').append(select).append("<span>رکورد</span>"));
            controls.append(length);
        }

        // ردیف خالی Bootstrap که بعد از انتقال جست‌وجو و تعداد رکورد باقی می‌ماند حذف می‌شود.
        container.children(".row").filter(function () {
            return $(this).find("input, select, table, .dataTables_info, .dataTables_paginate").length === 0;
        }).remove();

        exports.append('<button type="button" class="dataTables-export dataTables-export--excel" data-grid-export="excel"><i class="ti-file"></i> خروجی Excel</button>');
        exports.append('<button type="button" class="dataTables-export dataTables-export--word" data-grid-export="word"><i class="ti-files"></i> خروجی Word</button>');
        container.prepend(toolbar);
        toolbar.on("click", "[data-grid-export]", function () {
            openColumnModal(api, this.dataset.gridExport);
        });
    }

    function initializeTable(table) {
        if (!shouldEnhance(table)) { return; }
        table.dataset.datatableInitialized = "true";
        ensureUniqueId(table);

        var api;
        if ($.fn.dataTable.isDataTable(table)) {
            api = $(table).DataTable();
        } else {
            api = $(table).DataTable({
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "همه"]],
                order: [],
                language: persianLanguage
            });
        }
        createToolbar(api);
        api.columns.adjust();
        if (api.responsive && typeof api.responsive.recalc === "function") { api.responsive.recalc(); }
    }

    function ensureModal() {
        if (document.getElementById(modalId)) { return; }
        var html = '<div class="modal fade grid-export-modal" id="' + modalId + '" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-dialog-centered" role="document"><div class="modal-content">' +
            '<div class="modal-header"><h5 class="modal-title">انتخاب ستون‌های خروجی</h5><button type="button" class="close" data-dismiss="modal" aria-label="بستن"><i class="ti-close"></i></button></div>' +
            '<div class="modal-body"><p class="grid-export-help">ستون‌های موردنظر را انتخاب کنید. اگر هیچ ستونی انتخاب نشود، همه ستون‌ها خروجی گرفته می‌شوند.</p>' +
            '<div class="grid-column-actions"><button type="button" data-columns-all>انتخاب همه</button><button type="button" data-columns-clear>پاک کردن انتخاب</button></div><div class="grid-column-list"></div></div>' +
            '<div class="modal-footer"><button type="button" class="btn btn-secondary" data-dismiss="modal">انصراف</button><button type="button" class="btn btn-primary" data-export-confirm>دریافت فایل</button></div>' +
            '</div></div></div>';
        $(document.body).append(html);
        var modal = $("#" + modalId);
        modal.on("click", "[data-columns-all]", function () { modal.find(".grid-column-option input").prop("checked", true); });
        modal.on("click", "[data-columns-clear]", function () { modal.find(".grid-column-option input").prop("checked", false); });
        modal.on("click", "[data-export-confirm]", function () {
            if (!activeExport) { return; }
            var selected = modal.find(".grid-column-option input:checked").map(function () { return Number(this.value); }).get();
            if (!selected.length) { selected = activeExport.columns.map(function (column) { return column.index; }); }
            downloadTable(activeExport.api, selected, activeExport.type);
            modal.modal("hide");
        });
    }

    function openColumnModal(api, type) {
        ensureModal();
        var columns = exportableColumns(api);
        activeExport = { api: api, type: type, columns: columns };
        var list = $("#" + modalId + " .grid-column-list").empty();
        columns.forEach(function (column, position) {
            var id = "gridExportColumn_" + position;
            list.append('<label class="grid-column-option" for="' + id + '"><input id="' + id + '" type="checkbox" value="' + column.index + '"><span>' + $("<div>").text(column.title).html() + '</span></label>');
        });
        $("#" + modalId).modal("show");
    }

    function buildExportTable(api, selectedColumns) {
        var headers = selectedColumns.map(function (index) { return cleanText(api.column(index).header().innerHTML); });
        var rows = [];
        api.rows({ search: "applied" }).every(function () {
            var node = this.node();
            var cells = node ? node.cells : [];
            rows.push(selectedColumns.map(function (index) { return cleanText(cells[index] ? cells[index].innerHTML : ""); }));
        });
        var escape = function (value) { return $("<div>").text(value).html(); };
        var head = "<tr>" + headers.map(function (title) { return "<th>" + escape(title) + "</th>"; }).join("") + "</tr>";
        var body = rows.map(function (row) { return "<tr>" + row.map(function (cell) { return "<td>" + escape(cell) + "</td>"; }).join("") + "</tr>"; }).join("");
        return '<table border="1" dir="rtl"><thead>' + head + '</thead><tbody>' + body + '</tbody></table>';
    }

    function downloadTable(api, selectedColumns, type) {
        var title = (document.title || "گزارش").replace(/[\\/:*?"<>|]/g, "-");
        var table = buildExportTable(api, selectedColumns);
        var isWord = type === "word";
        var content = '<html><head><meta charset="utf-8"><style>body{font-family:Tahoma;direction:rtl}table{border-collapse:collapse;width:100%}th,td{padding:7px;text-align:right}th{background:#e9eef8}</style></head><body><h3>' + title + '</h3>' + table + '</body></html>';
        var blob = new Blob(["\ufeff", content], { type: isWord ? "application/msword;charset=utf-8" : "application/vnd.ms-excel;charset=utf-8" });
        var url = URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.href = url;
        link.download = title + (isWord ? ".doc" : ".xls");
        document.body.appendChild(link);
        link.click();
        link.remove();
        setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
    }

    function scan(root) {
        var tables = root.matches && root.matches("table") ? [root] : root.querySelectorAll ? root.querySelectorAll("table") : [];
        Array.prototype.forEach.call(tables, initializeTable);
    }

    /** عرض ستون‌های جدول‌های قابل مشاهده را پس از تغییر اندازه، تب یا مودال اصلاح می‌کند. */
    function adjustVisibleTables() {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
        $.fn.dataTable.tables({ visible: true, api: true }).responsive.recalc();
    }

    $(function () {
        ensureModal();
        scan(document);
        var resizeTimer;
        $(window).on("resize.unifiedDataTables", function () {
            window.clearTimeout(resizeTimer);
            resizeTimer = window.setTimeout(adjustVisibleTables, 140);
        });
        $(document).on("shown.bs.tab shown.bs.modal", function () {
            window.setTimeout(adjustVisibleTables, 50);
        });
        if (window.MutationObserver) {
            new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    Array.prototype.forEach.call(mutation.addedNodes, function (node) { if (node.nodeType === 1) { scan(node); } });
                });
            }).observe(document.body, { childList: true, subtree: true });
        }
    });
})(window.jQuery);
