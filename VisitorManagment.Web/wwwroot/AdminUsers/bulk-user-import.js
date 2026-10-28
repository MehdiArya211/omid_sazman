(function () {
    'use strict';

    var table = document.getElementById('bulkImportTable');
    if (!table) return;

    var units = Array.isArray(window.bulkImportUnits) ? window.bulkImportUnits : [];
    var unitMap = {};
    units.forEach(function (unit) { unitMap[normalize(unit.title)] = unit.code; });

    function normalize(value) {
        return (value || '').replace(/ي/g, 'ی').replace(/ك/g, 'ک').replace(/\u200c/g, ' ').replace(/\s+/g, ' ').trim();
    }

    function updateUnitCode(input) {
        var row = input.closest('[data-import-row]');
        if (!row) return;
        var code = unitMap[normalize(input.value)] || '';
        row.querySelector('.unit-code').value = code;
        row.classList.toggle('has-warning', !code);
    }

    function reindexRows() {
        table.querySelectorAll('tbody [data-import-row]').forEach(function (row, index) {
            row.querySelector('.row-number').textContent = index + 1;
            row.querySelectorAll('[name]').forEach(function (field) {
                field.name = field.name.replace(/Rows\[\d+\]/, 'Rows[' + index + ']');
                if (field.id) field.id = field.id.replace(/Rows_\d+__/, 'Rows_' + index + '__');
            });
            var numberInput = row.querySelector('[name$=".RowNumber"]');
            if (numberInput) numberInput.value = index + 1;
        });
    }

    table.addEventListener('change', function (event) {
        if (event.target.classList.contains('unit-title')) updateUnitCode(event.target);
    });

    table.addEventListener('click', function (event) {
        var button = event.target.closest('.remove-import-row');
        if (!button) return;
        button.closest('[data-import-row]').remove();
        reindexRows();
    });

    var selectAll = document.getElementById('selectAllRows');
    if (selectAll) selectAll.addEventListener('change', function () {
        table.querySelectorAll('.row-selector:not(:disabled)').forEach(function (checkbox) { checkbox.checked = selectAll.checked; });
    });

    var addButton = document.getElementById('addManualRow');
    if (addButton) addButton.addEventListener('click', function () {
        var source = table.querySelector('tbody [data-import-row]');
        if (!source) return;
        var row = source.cloneNode(true);
        row.className = 'has-warning';
        row.querySelectorAll('input').forEach(function (input) {
            input.disabled = false;
            if (input.type === 'checkbox') input.checked = true;
            else input.value = '';
        });
        row.querySelectorAll('select').forEach(function (select) { select.value = '0'; });
        row.querySelector('.status-title').textContent = 'ورود دستی';
        var statusCell = row.querySelector('.status-cell');
        var oldMessage = statusCell.querySelector('small');
        if (oldMessage) oldMessage.remove();
        table.querySelector('tbody').appendChild(row);
        reindexRows();
        row.querySelector('input:not([type="hidden"])').focus();
    });

    var form = document.getElementById('bulkImportForm');
    if (form) form.addEventListener('submit', function () {
        reindexRows();
        form.querySelectorAll('.row-selector:disabled').forEach(function (checkbox) { checkbox.checked = false; checkbox.disabled = false; });
    });
}());
