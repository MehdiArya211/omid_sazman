$(document).on('click', 'a.tour', function () {
    var enjoyhint_instance = new EnjoyHint({});

    enjoyhint_instance.set([
        {
            'next .navigation-menu-body': 'از لیست منو میتوانید به بخش های مختلفی از سامانه بروید  * توجه اگر بخش منو قابل مشاهده نبود دکمه کنترلو منها را نگه دارید',
        },
        {
            'next #cardContent': 'این قسمت آمار کلی ملاقات ها را به تفکیک اقدام شد ، ثبت نظریه و عودت نمایش می دهد  ',
        },

        {
            'next #level': 'از این قسمت می توانید رتبه یگان خود را مشاهده کنید',
        }, 
 
    ]);
    enjoyhint_instance.run();

    return false;
});

