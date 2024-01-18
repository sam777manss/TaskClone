function ValidateLogin() {

    var Email = $('#email').val();
    var Password = $('#your_pass').val();

    if (Email == "") {
        var input = $('input[name="email"]');
        input.attr('placeholder', 'Enter Email');
        $('label[for="email"]').css('color', 'red');
        return false;
    }

    // if var Email = "" then Email == "" and if var Email = null then Email != null
    if (Email != "" || Email != null) {
        var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;

        if ( !pattern.test(Email) ) {
            //alert('not a valid e-mail address');
            return false;
        }
    }
    if (Password == "") {
        var input = $('input[name="Password"]');
        input.attr('placeholder', 'Enter Password');
        $('label[for="your_pass"]').css('color', 'red');
        return false;
    }

    return true
}
