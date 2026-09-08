(function () {
    'use strict';

    var draggedNode = null;
    var tree = document.querySelector('.menu-tree');
    if (!tree) return;

    function directList(node) {
        return Array.prototype.find.call(node.children, function (child) {
            return child.matches && child.matches('[data-menu-list]');
        });
    }

    tree.addEventListener('dragstart', function (event) {
        draggedNode = event.target.closest('.menu-node');
        if (!draggedNode) return;
        draggedNode.classList.add('is-dragging');
        event.dataTransfer.effectAllowed = 'move';
        event.dataTransfer.setData('text/plain', draggedNode.dataset.menuId);
    });

    tree.addEventListener('dragend', function () {
        if (draggedNode) draggedNode.classList.remove('is-dragging');
        draggedNode = null;
        tree.querySelectorAll('.is-drop-target').forEach(function (item) { item.classList.remove('is-drop-target'); });
    });

    tree.addEventListener('dragover', function (event) {
        var list = event.target.closest('[data-menu-list]');
        var target = event.target.closest('.menu-node');
        if (!draggedNode || !list || draggedNode.contains(list)) return;
        if (directList(draggedNode) && directList(draggedNode).children.length && list.dataset.parentId) return;
        event.preventDefault();
        event.dataTransfer.dropEffect = 'move';
        tree.querySelectorAll('.is-drop-target').forEach(function (item) { item.classList.remove('is-drop-target'); });
        (target || list).classList.add('is-drop-target');
    });

    tree.addEventListener('drop', function (event) {
        var list = event.target.closest('[data-menu-list]');
        var target = event.target.closest('.menu-node');
        if (!draggedNode || !list || draggedNode.contains(list)) return;
        if (directList(draggedNode) && directList(draggedNode).children.length && list.dataset.parentId) return;
        event.preventDefault();
        if (target && target !== draggedNode && target.parentElement === list) {
            var rect = target.getBoundingClientRect();
            list.insertBefore(draggedNode, event.clientY < rect.top + rect.height / 2 ? target : target.nextSibling);
        } else {
            list.appendChild(draggedNode);
        }
        draggedNode.classList.toggle('menu-node--child', Boolean(list.dataset.parentId));
    });

    document.querySelectorAll('[data-menu-edit]').forEach(function (button) {
        button.addEventListener('click', function () {
            var modal = document.getElementById('menuEditModal');
            var form = modal.querySelector('form');
            ['permissionId', 'title', 'parentId', 'parentUrl', 'subUrl', 'menuUrl', 'iconName', 'order'].forEach(function (name) {
                var field = form.elements[name];
                var key = name === 'permissionId' ? 'id' : name.replace(/[A-Z]/g, function (letter) { return '-' + letter.toLowerCase(); });
                if (field) field.value = button.dataset[key] || '';
            });
            form.elements.isActive.checked = button.dataset.active === 'true';
            form.elements.showAll.checked = button.dataset.showAll === 'true';
            Array.prototype.forEach.call(form.elements.parentId.options, function (option) {
                option.disabled = option.value === button.dataset.id;
            });
            if (window.jQuery) window.jQuery(modal).modal('show');
        });
    });

    var saveButton = document.querySelector('[data-save-menu-order]');
    saveButton.addEventListener('click', function () {
        var originalContent = saveButton.innerHTML;
        var items = [];
        document.querySelectorAll('[data-menu-list]').forEach(function (list) {
            Array.prototype.forEach.call(list.children, function (node, index) {
                if (!node.matches('.menu-node')) return;
                items.push({ permissionId: Number(node.dataset.menuId), parentId: list.dataset.parentId ? Number(list.dataset.parentId) : null, order: index + 1 });
            });
        });
        var token = document.querySelector('[data-antiforgery] input[name="__RequestVerificationToken"]');
        saveButton.disabled = true;
        saveButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span><span>در حال ذخیره...</span>';
        fetch(window.location.pathname + '?handler=SaveOrder', {
            method: 'POST', credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token ? token.value : '' },
            body: JSON.stringify(items)
        }).then(function (response) { return response.json(); })
          .then(function (result) {
              if (!result.success) throw new Error(result.message);
              if (window.Swal) Swal.fire({ icon: 'success', title: 'ذخیره شد', text: result.message, confirmButtonText: 'باشه' });
              else alert(result.message);
          }).catch(function (error) {
              var message = error.message || 'ذخیره چیدمان انجام نشد.';
              if (window.Swal) Swal.fire({ icon: 'error', title: 'خطا', text: message, confirmButtonText: 'باشه' });
              else alert(message);
          }).finally(function () {
              saveButton.disabled = false;
              saveButton.innerHTML = originalContent;
          });
    });
}());
