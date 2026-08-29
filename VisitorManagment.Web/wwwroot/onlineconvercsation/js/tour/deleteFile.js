$(document).on('click', 'a.deleteFileTour', function () {
    var enjoyhint_instance = new EnjoyHint({});

    enjoyhint_instance.set([
        {
            'next #deleteFileSection1': 'در این قسمت میتوان درخواست ملاقات خود را حذف کرد',
        } 
    ]);
    enjoyhint_instance.run();

    return false;
});

