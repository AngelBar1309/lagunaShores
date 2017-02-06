//Function to save in session memory in server the aside status
function saveAsideStatus(asideStatus) {
    $.ajax(
        {
            url: "/Home/saveAsideStatus",
            method: "POST",
            data: { asideStatus: asideStatus },
            dataType: "html"
        })
}

//Get exchange rate from OpenExchangeRates API given currency and method to handle request result.
function getExchangeRateUSDto(currency,handler) {
    url = "https://openexchangerates.org/api/latest.json?app_id=a9eca34469cd4bb192863fb5e525a534";
    $.ajax(
        {
            url: url,
            method: "GET",
            dataType: "JSON",
            success:function (result) {
                somVal = (Math.floor((result.rates[currency]) * 100) / 100).toString();
            },
            error:function () {
                somVal = "";
            },
            complete:function () {
                handler(somVal);
            }
        }
    )
}

//Adds new attachments comment in crea sales contract screen
var contador = 0;
var BonusNightEspañol = "NOCHES BONO CADA AÑO POR  AÑOS, SUJETO A" +
                                    "DISPONIBILIDAD, NO FINES DE SEMANA, NO DIAS FESTIVOS, SOLO DE" +
                                    "LUNES A JUEVES." +
                                    "\n 1 REC OCOTILLO $50.00 USD POR NOCHE." +
                                    "\n 2 REC VENTANAS $75.00 USD POR NOCHE." +
                                    "\n 2 REC VILLA PLAYA $100.00 USD POR NOCHE." +
                                    "\n 3 REC VILLA PLAYA $120.00 USD POR NOCHE.";
var BonusNightIngles = "BONUS NIGHTS EACH YEAR FOR  YEARS, DEPENDING ON AVAILABILITY," +
                                    "NO WEEKENDS, NO HOLIDAYS, ONLY MONDAY THRU THURSDAY." +
                                    "\n 1 BDR OCOTILLO $50.00 USD PER NIGHT." +
                                   " \n 2 BDR VENTANAS $75.00 USD PER NIGHT." +
                                    "\n 2 BDR VILLA PLAYA $100.00 USD PER NIGHT." +
                                    "\n 3 BDR VILLA PLAYA $120.00 USD PER NIGHT.";
$("#BtnAgregarAtachmentEsp").click(function () { //For spanish attachments comments
    var valorSelectEspañol = $("#SpanishAttachmentSelector").val().split("|");
    var textAreaEspañol = $("#attachmentsSpanish").val();
    var textAreaIngles = $("#attachmentsEnglish").val();
    //Si el Atachment seleccionado es el de "Bonus Nights" se agrega el texto de bonusNightEspañol e bonusNightIngles
    //si se agarra el texto que tiene adentro el <Selected> en la vista no funciona los saltos de linea
    if ($('#SpanishAttachmentSelector :selected').text() == "Bonus Nights") {
        $("#attachmentsSpanish").val(textAreaEspañol + BonusNightEspañol + "\n");
        $("#attachmentsEnglish").val(textAreaIngles + BonusNightIngles + "\n");
    }
    else {
        $("#attachmentsSpanish").val(textAreaEspañol + valorSelectEspañol[1] + "\n");
        $("#attachmentsEnglish").val(textAreaIngles + valorSelectEspañol[0] + "\n");
        }
})
$("#BtnAgrearAtachmentIng").click(function () {//For english attachments comments
    var valorSelectIngles = $("#EnglishAttachmentSelector").val().split("|");
    var textAreaIngles = $("#attachmentsEnglish").val();
    var textAreaEspañol = $("#attachmentsSpanish").val();
    //Si el Atachment seleccionado es el de "Bonus Nights" se agrega el texto de bonusNightEspañol e bonusNightIngles
    //si se agarra el texto que tiene adentro el <Selected> en la vista no funciona los saltos de linea
    if ($('#EnglishAttachmentSelector :selected').text() == "Bonus Nights")
    {
        $("#attachmentsSpanish").val(textAreaEspañol + BonusNightEspañol + "\n");
        $("#attachmentsEnglish").val(textAreaIngles + BonusNightIngles + "\n");
    }
    else
    {
        $("#attachmentsSpanish").val(textAreaEspañol + valorSelectIngles[1] + "\n");
        $("#attachmentsEnglish").val(textAreaIngles + valorSelectIngles[0] + "\n");
    }
})



//Code to be able to add new sales members to a sales contract dinamically in the same screen
var max_fields = 6; //maximum input boxes allowed
var wrapper = $(".input_fields_wrap #aditionalSalesMembersSelector"); //Fields wrapper
var add_button = $(".add_field_button"); //Add button ID

var x = 1; //initlal text box count
var salesMemberSelectorPanel;
$(add_button).click(function (e) { //on add input button click
    e.preventDefault();
    //Takes information from the first empleoyee and role selectors
    //newSalesMemberSelector = $(".sale_member_selector").first().clone().css("display","none");
    newSalesMemberSelector = salesMemberSelectorPanel.clone();
    if (x < max_fields) { //max input box allowed
        x++; //text box increment
        //removeButton = $('<a href="#" class="remove_field remove_field btn btn-default">Remove</a>');
        newSalesMemberSelector.appendTo(wrapper);
        //removeButton.insertBefore(newSalesMemberSelector.find("#loader_animation"));
        newSalesMemberSelector.find("#SalesMembers").change(loadAndShowQualifications)
        newSalesMemberSelector.slideDown(500);
    }
});

//Get exchange rate from OpenExchangeRates API given currency and method to handle request result.
function getSalesMemberQualifications(salesMemberID, handler) {
    url = "/API/getQualifications";
    $.ajax(
        {
            url: url,
            method: "GET",
            dataType: "JSON",
            data: {salesMemeberID : salesMemberID}
        }).success(function (result) {
            handler(result);
        }).error(function () {
            handler(result);
        })
}

//Initialize functionalities for Sales Members in contracts in edition and creation.
var emptyRolesDropdown;
function initializeSalesMemberSelection() {
    salesMemberSelectorPanel = $("#aditionalSalesMembersSelector .sale_member_selector").first().clone().css("display", "none");
    salesMemberSelectorPanel.children('#memberSelection').find('select#SalesMembers').
        children('option[selected="selected"]').removeAttr('selected');
    salesMemberSelectorPanel.children('#rolSelection').find('select#SalesMemberTypes').
        empty().append($("<option></option>").html("Select Rol..."))

    $('#aditionalSalesMembersSelector select#SalesMembers').change(loadAndShowQualifications);
}

//Load async from server qualifications for specified sales member.
function loadAndShowQualifications() {
    selectedMememberID = $(this).val();
    rowSaleMemberSelector = $(this).closest(".sale_member_selector.row")
    loader = rowSaleMemberSelector.find('#loader_animation');
    loader.fadeIn();
    dropDownRoles = rowSaleMemberSelector.find("#rolSelection").find("select");
    dropDownRoles.empty();
    dropDownRoles.prop('disabled', true);
    getSalesMemberQualifications(selectedMememberID, function (qualificationsList) {
        loader.fadeOut();
        dropDownRoles.prop('disabled', false);
        if (qualificationsList != 0) {
            $.each(qualificationsList, function (index, qualification) {
                newRolOption = $("<option></option>").
                    val(qualification.rolID).html(qualification.type);
                newRolOption.appendTo(dropDownRoles);
            })
        }
        else {
            newRolOption = $("<option></option>").html("Select Rol...");
            newRolOption.appendTo(dropDownRoles);
        }
    })
}

//Code to remove a dianmically added field, used in create sales contract screen
$(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
    e.preventDefault();
    $(this).parent('div').slideUp(500, function () { $(this).remove(); })
    x--;
})
//Aqui se agrega un nuevo campo si el type of fraction es diferente a las opciones que se ofrece al crear un contrato
//Si un 1/100 entonces se muestra un nuevo campo boleano
var divAbierto;
$('#typeOfFraction').on('change', function () {
    //Cuando se cambia se escoge otra opcion si el div que se mostraba era DivOther entonces se elimina 
    var divTypeofFraction = $('#typeOfFractionOther').val();
   
    if (divAbierto === "divOther" || divTypeofFraction!=null) {
        $('#typeOfFractiondiv input').append().slideUp(500, function () {
            $(this).remove();

        })
    }
    //Si era el YearEvenDiv entonces se elimina este
    if (divAbierto === "YearEvenDiv")
    {
        $('#yearEvenSection').css("display", "none")
    }
    //Se agrega div Other
    if ($(this).val() === "Other") {

        typeOfFractiondiv = $('#typeOfFractiondiv'); 
        newinput = $("<input type='text' id='typeOfFractionOther' name='typeOfFractionOther' class='form-control'placeholder='Specify Type Of Fraction'/>");
        newinput.appendTo(typeOfFractiondiv.slideDown("slow"));
        divAbierto = "divOther";
    }
    //se agrega div YearEven
    if ($(this).val() === "1/100")
    {
        $('#yearEvenSection').css("display", "Block")
        divAbierto = "YearEvenDiv";
    }
   
});

