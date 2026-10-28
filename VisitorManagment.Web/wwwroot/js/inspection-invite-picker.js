/* انتخاب یگان و نفرات جلسه؛ داده‌ها در همان checkboxهای فرم نگهداری می‌شوند. */
window.initializeInspectionInvitePicker = function (units, people) {
    const form = document.getElementById('meetingForm');
    const unitList = document.getElementById('unitList'), personList = document.getElementById('personList');
    const unitHeader = unitList.previousElementSibling, peopleHeader = personList.previousElementSibling;
    const workspace = document.createElement('section');
    workspace.className = 'invite-workspace';
    const unitPanel = document.createElement('section'), peoplePanel = document.createElement('section');
    unitPanel.className = 'invite-panel invite-panel--units'; peoplePanel.className = 'invite-panel invite-panel--people';
    unitHeader.before(workspace); workspace.append(unitPanel, peoplePanel);
    unitPanel.append(unitHeader, unitList); peoplePanel.append(peopleHeader, personList);
    const heading = peopleHeader.querySelector('h5');
    const empty = document.createElement('div'); empty.className = 'invite-empty'; personList.after(empty);
    const footer = document.createElement('div'); footer.className = 'invite-footer';
    const summary = document.createElement('strong'), review = document.createElement('button');
    review.type = 'button'; review.className = 'btn btn-outline-info'; review.textContent = 'مشاهده دعوت‌شدگان';
    const save = document.getElementById('saveMeeting'), publish = form.querySelector('.inspection-switch');
    save.before(footer); footer.append(summary, review, publish, save);
    const dialog = document.createElement('dialog'); dialog.className = 'invite-review';
    dialog.innerHTML = '<header><h4>بازبینی دعوت‌شدگان</h4><button type="button" class="btn btn-light">بستن</button></header><div class="invite-review-body"></div>';
    document.body.append(dialog); dialog.querySelector('button').onclick = () => dialog.close();
    dialog.addEventListener('click', e => { if(e.target === dialog) dialog.close(); });
    let activeUnit = units.find(x => x.checked)?.value || units[0]?.value;
    const normalize = value => String(value || '').replace(/ي/g,'ی').replace(/ك/g,'ک').replace(/[۰-۹]/g,d=>'۰۱۲۳۴۵۶۷۸۹'.indexOf(d)).trim().toLowerCase();
    const unitSearch = document.getElementById('unitSearch'), personSearch = document.getElementById('personSearch');
    units.forEach(input => {
        const label = input.closest('label'), open = document.createElement('button');
        open.type = 'button'; open.className = 'invite-open-unit'; open.textContent = 'نفرات';
        label.append(open);
        const badge = document.createElement('small'); badge.className = 'invite-unit-count'; label.append(badge);
        open.onclick = event => { event.preventDefault(); activeUnit = input.value; refresh(); };
        input.addEventListener('change', () => { activeUnit = input.value; refresh(); });
    });
    function refresh() {
        const selectedUnits = new Set(units.filter(x=>x.checked).map(x=>x.value));
        units.forEach(input => {
            const label = input.closest('label'), members = people.filter(p=>p.dataset.unitCode===input.value);
            label.classList.toggle('is-current',input.value===activeUnit);
            label.hidden = !normalize(label.textContent).includes(normalize(unitSearch.value));
            label.querySelector('.invite-unit-count').textContent = members.filter(p=>p.querySelector('input').checked).length + ' منتخب';
        });
        people.forEach(label => {
            label.hidden = label.dataset.unitCode!==activeUnit || !normalize(label.textContent).includes(normalize(personSearch.value));
            label.querySelector('input').disabled = !selectedUnits.has(label.dataset.unitCode);
        });
        const active = units.find(x=>x.value===activeUnit);
        heading.textContent = active ? 'نفرات «' + active.closest('label').querySelector('b').textContent + '»' : 'نفرات شرکت‌کننده';
        empty.hidden = people.some(p=>!p.hidden) && selectedUnits.has(activeUnit);
        empty.textContent = !selectedUnits.has(activeUnit) ? 'برای انتخاب نفرات، ابتدا تیک دعوت این یگان را بزنید.' : 'نفری با این مشخصات یافت نشد.';
        const total = people.filter(p=>selectedUnits.has(p.dataset.unitCode)&&p.querySelector('input').checked).length;
        summary.textContent = selectedUnits.size + ' یگان دعوت‌شده · ' + total + ' نفر انتخاب‌شده';
        review.disabled = !selectedUnits.size;
    }
    function setVisible(checked) {
        people.filter(p=>!p.hidden&&!p.querySelector('input').disabled).forEach(p=>p.querySelector('input').checked=checked); refresh();
    }
    document.getElementById('selectAllPeople').textContent = 'انتخاب نتایج این یگان';
    document.getElementById('clearAllPeople').textContent = 'پاک‌کردن نتایج';
    document.getElementById('selectAllPeople').onclick = () => setVisible(true);
    document.getElementById('clearAllPeople').onclick = () => setVisible(false);
    unitSearch.oninput = refresh; personSearch.oninput = refresh;
    people.forEach(p=>p.querySelector('input').addEventListener('change',refresh));
    review.onclick = () => {
        const body = dialog.querySelector('.invite-review-body'); body.replaceChildren();
        units.filter(x=>x.checked).forEach(unit=>{
            const section=document.createElement('section'),title=document.createElement('h5');
            title.textContent=unit.closest('label').querySelector('b').textContent; section.append(title);
            const members=people.filter(p=>p.dataset.unitCode===unit.value&&p.querySelector('input').checked);
            if(!members.length){const note=document.createElement('p');note.textContent='هیچ نفری انتخاب نشده است.';section.append(note);}
            members.forEach(p=>{
                const row=document.createElement('div'),name=document.createElement('span'),remove=document.createElement('button');
                row.className='invite-review-person';name.textContent=p.textContent.trim();remove.type='button';remove.className='btn btn-sm btn-outline-danger';remove.textContent='حذف';
                remove.onclick=()=>{p.querySelector('input').checked=false;row.remove();refresh();};
                row.append(name,remove);section.append(row);
            });body.append(section);
        });dialog.showModal();
    };
    // checkbox غیرفعال یگان از فرم ارسال نمی‌شود؛ انتخاب‌های آن برای بازگشت احتمالی محفوظ می‌ماند.
    refresh(); return refresh;
};
