﻿var lastZipRequest = '';
var lastCvv2Request = '';
var lastPasswordRequest = '';
var lastCCNumberRequest = '';


function onZipKeyUp(e)
{
    var requestValue = postBackOnKeyUp(e, 5, lastZipRequest);
    if (requestValue != '')
    {
        lastZipRequest = requestValue;
    }
}

function validateLength(id, length)
{
    var ele = document.getElementById(id);
    if (ele == null)
    {
        return false;
    }
    return ele.value.length >= length;
}

function onCvv2KeyUp(e, cardTypeId)
{
    var cvv2Length = 3;
    var cardType = document.getElementById(cardTypeId);
    if (cardType.value == 'AmericanExpress')
    {
        cvv2Length = 4;
    }
    var requestValue = postBackOnKeyUp(e, cvv2Length, lastCvv2Request);
    if (requestValue != '')
    {
        lastCvv2Request = requestValue;
    }
}


function onCardNumberKeyUp(e, cardTypeId, securityCodeId, nameOnCardId)
{
    if (!validateLength(cardTypeId, 4))
    {
        return;
    }
    if (!validateLength(securityCodeId, 3))
    {
        return;
    }
    if (!validateLength(nameOnCardId, 2))
    {
        return;
    }

    var requestValue = postBackOnKeyUp(e, 16, lastCCNumberRequest);
    if (requestValue != '')
    {
        lastCCNumberRequest = requestValue;
    }
}

function onCreateAccountPasswordKeyUp(e, passwordBoxId, passwordConfirmId)
{
    var passwordBox = document.getElementById(passwordBoxId);
    var passwordConfirm = document.getElementById(passwordConfirmId);

    if (passwordBox.value.length == passwordConfirm.value.length)
    {
        if (Page_ClientValidate('VGCreateAccount'))
        {
            var requestValue = postBackOnKeyUp(e, document.getElementById(passwordBoxId).value.length, lastPasswordRequest);
            if (requestValue != '')
            {
                lastPasswordRequest = requestValue;
            }
        }
    }
}

function postBackOnKeyUp(theEvent, minLength, lastRequestValue)
{
    var eventTarget = theEvent.target;
    if (eventTarget === undefined)
    {
        eventTarget = theEvent.srcElement;
    }
    var eventValue = eventTarget.value;

    if ((eventValue.length == minLength) && (eventValue != lastRequestValue))
    {
        __doPostBack(eventTarget.id.toString());
        return eventValue;
    }

    return '';
}

function Over13Check(sender, args) {
    //validate a single checkbox
    var chkControlId = sender.id.replace("validate_", "");
    var option = document.getElementById(chkControlId);
    args.IsValid = false;
    
    if (option.type == "checkbox") {
        if (option.checked) {
            args.IsValid = true;
        }
    }
}



