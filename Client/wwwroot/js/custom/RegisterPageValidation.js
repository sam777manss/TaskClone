function ValidateRegiter() {

    var Name = $('#name').val();
    var Email = $('#email').val();
    var Password = $('#pass').val();
    var ConfirmPassword = $('#re_pass').val();
    if (Name == "") {
        var input = $('input[name="name"]');
        input.attr('placeholder', "Enter Name");
        $('label[for="name"]').css('color', 'red');
        return false;
    }
    if (Email == "") {
        var input = $('input[name="email"]');
        input.attr('placeholder', "Enter Email");
        $('label[for="email"]').css('color', 'red');
        return false;
    }
     //if var Email = "" then Email == "" and if var Email = null then Email != null
    if (Email != "") {
        var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;

        if (!pattern.test(Email)) {
            //alert('not a valid e-mail address');
            return false;
        }
    }
    if (Password == "") {
        var input = $('input[name="password"]');
        input.attr('placeholder', "Enter Password");
        $('label[for="pass"]').css('color', 'red');
        return false;
    }
    if (ConfirmPassword == "") {
        var input = $('input[name="ConfirmPassword"]');
        input.attr('placeholder', "Enter Confirm Password");
        $('label[for="re-pass"]').css('color', 'red');
        return false;
    }

    if (Password != ConfirmPassword) {
        return false;
    }

    return true;
}
