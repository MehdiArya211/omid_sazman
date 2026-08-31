(function () {
    "use strict";

    function chartForCanvas(canvas) {
        if (!window.Chart || !Chart.instances) { return null; }
        var instances = Chart.instances;
        var keys = Array.isArray(instances) ? instances.keys() : Object.keys(instances);
        var result = null;
        for (var key of keys) {
            var chart = Array.isArray(instances) ? instances[key] : instances[key];
            if (chart && ((chart.chart && chart.chart.canvas === canvas) || chart.canvas === canvas)) { result = chart; break; }
        }
        return result;
    }

    function enhanceChart(canvas) {
        if (canvas.dataset.chartControls === "true") { return; }
        var chart = chartForCanvas(canvas);
        if (!chart) { return; }
        canvas.dataset.chartControls = "true";
        var toolbar = document.createElement("div");
        toolbar.className = "report-chart-toolbar";
        toolbar.innerHTML = '<label>نوع نمودار</label><select><option value="bar">ستونی</option><option value="line">خطی</option><option value="pie">دایره‌ای</option><option value="doughnut">حلقه‌ای</option></select>';
        canvas.parentNode.insertBefore(toolbar, canvas);
        var select = toolbar.querySelector("select");
        select.value = ["bar", "line", "pie", "doughnut"].indexOf(chart.config.type) >= 0 ? chart.config.type : "bar";
        var originalScales = chart.options && chart.options.scales;
        select.addEventListener("change", function () {
            var current = chartForCanvas(canvas) || chart;
            var data = current.data;
            var options = current.options || {};
            current.destroy();
            if (select.value === "pie" || select.value === "doughnut") {
                if (options.scales) { originalScales = options.scales; delete options.scales; }
            } else if (!options.scales && originalScales) {
                options.scales = originalScales;
            }
            chart = new Chart(canvas.getContext("2d"), { type: select.value, data: data, options: options });
        });
    }

    function scanCharts() {
        if (window.location.pathname.toLowerCase().indexOf("/admin/reports/") === -1) { return; }
        document.querySelectorAll("canvas").forEach(enhanceChart);
    }

    function normalizeDigits(value) {
        var persianDigits = "۰۱۲۳۴۵۶۷۸۹";
        var arabicDigits = "٠١٢٣٤٥٦٧٨٩";
        return (value || "").trim().replace(/[۰-۹٠-٩]/g, function (digit) {
            var index = persianDigits.indexOf(digit);
            return String(index >= 0 ? index : arabicDigits.indexOf(digit));
        }).replace(/-/g, "/");
    }

    function parsePersianDate(value) {
        var normalized = normalizeDigits(value);
        var match = /^(\d{4})\/(\d{1,2})\/(\d{1,2})$/.exec(normalized);
        if (!match) { return null; }
        var year = Number(match[1]);
        var month = Number(match[2]);
        var day = Number(match[3]);
        var maximumDay = month <= 6 ? 31 : (month <= 11 ? 30 : 30);
        if (year < 1200 || year > 1600 || month < 1 || month > 12 || day < 1 || day > maximumDay) { return null; }
        return { normalized: year + "/" + String(month).padStart(2, "0") + "/" + String(day).padStart(2, "0"), key: year * 10000 + month * 100 + day };
    }

    function showValidationError(message, input) {
        if (window.Swal && Swal.fire) {
            Swal.fire({ icon: "warning", title: "بازه گزارش معتبر نیست", text: message, confirmButtonText: "متوجه شدم" });
        } else if (window.toastr && toastr.warning) {
            toastr.warning(message);
        } else {
            window.alert(message);
        }
        if (input) { input.focus(); }
    }

    function validateReportSearch(event) {
        var form = event.target;
        if (!form || form.tagName !== "FORM") { return; }
        var startInput = form.querySelector('[name="startDateSearch"]');
        var endInput = form.querySelector('[name="endDateSearch"]');
        if (!startInput && !endInput) { return; }
        var start = startInput && startInput.value.trim() ? parsePersianDate(startInput.value) : null;
        var end = endInput && endInput.value.trim() ? parsePersianDate(endInput.value) : null;
        if (startInput && startInput.value.trim() && !start) {
            event.preventDefault();
            showValidationError("تاریخ شروع را به شکل 1405/01/01 وارد کنید.", startInput);
            return;
        }
        if (endInput && endInput.value.trim() && !end) {
            event.preventDefault();
            showValidationError("تاریخ پایان را به شکل 1405/01/01 وارد کنید.", endInput);
            return;
        }
        if (start && end && start.key > end.key) {
            event.preventDefault();
            showValidationError("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.", startInput);
            return;
        }
        if (startInput && start) { startInput.value = start.normalized; }
        if (endInput && end) { endInput.value = end.normalized; }
    }

    window.addEventListener("load", function () {
        setTimeout(scanCharts, 400);
        setTimeout(scanCharts, 1200);
    });
    document.addEventListener("submit", validateReportSearch, true);
})();
