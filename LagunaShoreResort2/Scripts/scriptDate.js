$(document).ready(function () {
    $('.calendarPicker').datepicker({ dateFormat: "dd/mm/yy" });

    $.validator.addMethod('date',
        function (value, element, params) {
            if (this.optional(element)) {
                return true;
            }

            var ok = true;
            try {
                $.datepicker.parseDate('dd/mm/yy', value);
            }
            catch (err) {
                ok = false;
            }
            return ok;
        });
});