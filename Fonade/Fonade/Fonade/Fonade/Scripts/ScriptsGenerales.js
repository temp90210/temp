
//Control de accion en las pestañas de la opcion Ver Proyecto
var pestanaActual = "";
function CargarPestana(nombreFrame, pagina) {
    if (pestanaActual != "") {
        document.getElementById(pestanaActual).src = "";
    }
    
    document.getElementById(nombreFrame).src = pagina;
    pestanaActual = nombreFrame;
}

function Confirmacion(texto) {
    if (!confirm(texto))
        return false;
   
}

/// Datapicker que nos sirve para mostrar un calendar 
// Creado por Diego Quiñonez
// 22-02-2014

(function ($) {

    $.fn.Datepicker = function(id,formato) {

        $(id).datepicker({
            dateFormat: formato,
            showOn: "button",
            buttonImage: "../../Images/calendar.png",
            changeYear: true,
            changeMonth: true,
            buttonImageOnly: true
        });
    };

})(jQuery);

(function ($) {

    $.fn.showModalDialog = function(url, width, height,resizable,scroll,center) {

        showModalDialog(url, "dialogWidth:" + width  + "px;dialogHeight:" + height 
              + "px;resizable:" + resizable + ";scroll:" + scroll + ";center:" + center);
    };

})(jQuery);

(function ($) {

    $.fn.windowopen = function(url, width, height,resizable,scroll,center) {

        window.open(url, '_blank', 'width=' + width + ',height=' + height + ',toolbar=no, scrollbars=' + scroll + ',resizable=no,center=yes;'); 
    };

})(jQuery);


/// Validamos los pendientes que tiene el proyecto para acreditar //
(function ($) {

    $.fn.windowopen = function (url, width, height, resizable, scroll, center) {

        window.open(url, '_blank', 'width=' + width + ',height=' + height + ',toolbar=no, scrollbars=' + scroll + ',resizable=no,center=yes;');
    };

})(jQuery);

(function ($) {

    $.fn.validarCheckbox = function (nombre) {

        var lMensaje;
        var lResult;

        var lPendiente = $(".chkPendiente").is(":checked");
        var lSubSanado = $(".chkSubsanado").is(":checked");
        var lAcreditado = $(".chkAcreditado").is(":checked");
        var lNoAcreditado = $(".chkNoAcreditado").is(":checked");
        var lAnexo1 = $(".ch1").is(":checked");
        var lAnexo2 = $(".chd2").is(":checked");
        var lAnexo3 = $(".ch3").is(":checked");
        var lDI = $(".chdi").is(":checked");


        switch (nombre) {
        case "Subsanado":
            if (lNoAcreditado) {
                lMensaje = "No puede activar el estado 'Subsanado' si el estado del emprendedor es  'No Acreditado'";
                lResult = false;
            }
            break;
        case "Acreditado":
            if (lNoAcreditado) {
                lMensaje = "No puede activar el estado 'Acreditado' si el estado del emprendedor es 'No Acreditado'";
                lResult = false;
            } else if (lPendiente && !lSubSanado) {
                lMensaje = "No puede activar el estado 'Acreditado' si el estado del emprendedor es 'Pendiente'";
                lResult = false;
            } else if (!lAnexo1 || !lAnexo2 || !lAnexo3 || !lDI) {
                lMensaje = "No puede activar el estado 'Acreditado' si los anexos no están completos (1,2,3 y DI)";
                lResult = false;
            }
            break;
        case "NoAcreditado":
            if (lAcreditado) {
                lMensaje = "No puede activar el estado 'No Acreditado' si el estado del emprendedor es 'Acreditado'";
                lResult = false;
            } else if (lSubSanado) {
                lMensaje = "No puede activar el estado 'No Acreditado' si el estado del emprendedor es 'Subsanado'";
                lResult = false;
            }
            break;
        case "Pendiente":
            if (lAcreditado) {
                lMensaje = "No puede activar el estado 'Pendiente' si el estado del emprendedor es 'Acreditado'";
                lResult = false;
            } else if (lNoAcreditado) {
                lMensaje = "No puede activar el estado 'Pendiente' si el estado del emprendedor es 'No Acreditado'";
                lResult = false;
            }
            break;
        }

        if (!lResult && lMensaje != "")
            alert(lMensaje);


        return lResult;
    };
}) (jQuery);






//  /*/






