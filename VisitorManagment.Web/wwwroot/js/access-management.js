(function () {
    "use strict";

    var toPersianNumber = function (value) {
        return value.toString().replace(/\d/g, function (digit) {
            return "۰۱۲۳۴۵۶۷۸۹"[digit];
        });
    };

    document.querySelectorAll("[data-role-picker]").forEach(function (picker) {
        picker.addEventListener("change", function () {
            if (picker.value && picker.value !== "0") {
                picker.form.submit();
            }
        });
    });

    document.querySelectorAll("[data-access-panel]").forEach(function (panel) {
        var items = Array.prototype.slice.call(panel.querySelectorAll("[data-access-item]"));
        var search = panel.querySelector("[data-access-search]");
        var selectAll = panel.querySelector("[data-select-all]");
        var counter = panel.querySelector("[data-selected-count]");
        var noResult = panel.querySelector("[data-no-result]");
        var action = panel.querySelector("[data-requires-selection]");

        var visibleItems = function () {
            return items.filter(function (item) { return item.style.display !== "none"; });
        };

        var updateState = function () {
            var visible = visibleItems();
            var checked = items.filter(function (item) { return item.querySelector("input[type=checkbox]").checked; });
            var visibleChecked = visible.filter(function (item) { return item.querySelector("input[type=checkbox]").checked; });
            counter.textContent = toPersianNumber(checked.length);
            action.disabled = checked.length === 0;
            selectAll.checked = visible.length > 0 && visibleChecked.length === visible.length;
            selectAll.indeterminate = visibleChecked.length > 0 && visibleChecked.length < visible.length;
        };

        items.forEach(function (item) {
            item.querySelector("input[type=checkbox]").addEventListener("change", updateState);
        });

        selectAll.addEventListener("change", function () {
            visibleItems().forEach(function (item) {
                item.querySelector("input[type=checkbox]").checked = selectAll.checked;
            });
            updateState();
        });

        search.addEventListener("input", function () {
            var phrase = search.value.trim().toLocaleLowerCase("fa");
            var matches = 0;
            items.forEach(function (item) {
                var isMatch = item.textContent.toLocaleLowerCase("fa").indexOf(phrase) !== -1;
                item.style.display = isMatch ? "flex" : "none";
                if (isMatch) { matches += 1; }
            });
            noResult.classList.toggle("is-visible", items.length > 0 && matches === 0);
            updateState();
        });

        updateState();
    });
})();
