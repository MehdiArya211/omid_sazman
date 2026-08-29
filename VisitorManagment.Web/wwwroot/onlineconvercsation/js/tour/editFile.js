$(document).on('click', 'a.editFileTour', function () {
    var enjoyhint_instance = new EnjoyHint({});

    enjoyhint_instance.set([
        {
            'next #editFileSection1': ' در این قسمت میتوان درخواست ملاقات خود را ویرایش کرد ',
        }

    ]);
    enjoyhint_instance.run();

    return false;
});

