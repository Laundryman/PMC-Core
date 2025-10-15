//Global vars for handling window open for the catalogue
var catalogueWindowRef = null;
var strWindowFeatures = "menubar=yes,resizable=yes,scrollbars=yes,status=yes,left=400,top=100,height=700,width=1200";
var _currentTab = 1;
//handle displaying catalogue in new window
function showCatalogue() {
    if (catalogueWindowRef === null || catalogueWindowRef.closed)
    /* if the pointer to the window object in memory does not exist
           or if such pointer exists but the window was closed */ {
        catalogueWindowRef = window.open("/home/catalogue.aspx", "catalogue", strWindowFeatures);
    } else {
        catalogueWindowRef.focus();
        /* else the window reference must exist and the window
           is not closed; therefore, we can bring it back on top of any other
           window with the focus() method. There would be no need to re-create
           the window or to reload the referenced resource. */
    };
}

$('#launch-catalogue').click(function () {
    showCatalogue();
});
//global var for original plano name
var currentEdit = { name: '', planoID: "0" };
var saveClicked = false;
var prevEdit = { name: '', planoID: "0" };
// trigger event when planogram name is clicked

$("body").on("click", ".plano-name-click", function (event) {

    //set previous edit values
    prevEdit["name"] = currentEdit["name"];
    prevEdit["planoID"] = currentEdit["planoID"];
    //save original name incase not saving
    currentEdit["name"] = $(this).prev(".planoName").val();
    currentEdit["planoID"] = $(this).prev(".planoName").attr("planoid");

    // hide div overlay then enable the input box
    $(this).hide().prev(".planoName").prop("disabled", false).focus();
    $(this).prev(".planoName").addClass("pname-edit"); //change the background color to indicate edit mode
    $(this).parent().siblings(".btn-save-plano-container").show();
    $(this).parent().siblings().find(".btn-save-planogram").click(function () {
        currentEdit["name"] = $(this).parent().prev().find(".planoName").val();
        currentEdit["planoId"] = 0;
        prevEdit["planoid"] = 0;
        saveClicked = true;
    });



});

$("body").click(function (event) {
    //Close the edit name box on clicking anywhere outside the table
    if (!saveClicked) {
        if ($(this).attr("class") !== "planogram-name") {
                $(".planoName[planoid=" + currentEdit["planoID"] + "]").val(currentEdit["name"]);
                $(".planoName[planoid=" + currentEdit["planoID"] + "]").next(".plano-name-click").show()
                $(".planoName[planoid=" + currentEdit["planoID"] + "]").parent().siblings(".btn-save-plano-container").hide();
                $(".planoName[planoid=" + currentEdit["planoID"] + "]").prop("disabled", true).removeClass('pname-edit'); saveClicked = false;
                prevEdit["name"] = "";
                prevEdit["planoID"] = 0;
                currentEdit["planoID"] = 0;
                currentEdit["name"] = "";
            }
        }
});

$("body").on("click", "div", function (event) {

    //handle the reset of the edit planogram name here - check if save has been clicked first ignore if it's just been opened
    if ($(this).attr("class") !== "plano-name-click") {
        if (!saveClicked) {
            if (!$(this).hasClass("planogram-name")) {
                if ($(this).find(".planoName").attr("planoid") !== currentEdit["planoID"]) {


                    $(".planoName[planoid=" + currentEdit["planoID"] + "]").val(currentEdit["name"]);
                    $(".planoName[planoid=" + currentEdit["planoID"] + "]").next(".plano-name-click").show()
                    $(".planoName[planoid=" + currentEdit["planoID"] + "]").parent().siblings(".btn-save-plano-container").hide();
                    $(".planoName[planoid=" + currentEdit["planoID"] + "]").prop("disabled", true).removeClass('pname-edit'); saveClicked = false;
                    prevEdit["name"] = "";
                    prevEdit["planoID"] = 0;
                    currentEdit["planoID"] = 0;
                    currentEdit["name"] = "";
                }
            }
            else {
                if ($(this).find(".planoName").attr("planoid") === currentEdit["planoID"]) {
                    event.stopPropagation();
                    $(this).find(".planoName").focus();
                }
            }
        }
    }
    else {
        if (prevEdit["planoId"] !== currentEdit["planoID"] && prevEdit["planoID"] !== 0) {
            $(".planoName[planoid=" + prevEdit["planoID"] + "]").val(prevEdit["name"]);
            $(".planoName[planoid=" + prevEdit["planoID"] + "]").next(".plano-name-click").show()
            $(".planoName[planoid=" + prevEdit["planoID"] + "]").parent().siblings(".btn-save-plano-container").hide();
            $(".planoName[planoid=" + prevEdit["planoID"] + "]").prop("disabled", true).removeClass('pname-edit'); saveClicked = false;
        }
        //    else 
        //    {
        //      prevEdit["planoID"] = $(this).prev(".planoName").attr("planoid");

        //    }
    }

});
//ACCORDION BUTTON ACTION (ON CLICK DO THE FOLLOWING)
$("body").on("click", '.expand-button', function (event) {

    //REMOVE THE ON CLASS FROM ALL BUTTONS
    $('.expand-button').removeClass('on');

    //NO MATTER WHAT, WE CLOSE ALL OPEN SLIDES
    $(".plano-action-container").slideUp('normal');
    $('.expand-button').children('.btn-open-close-plano').attr('src', '/images/btn-closed-planogram-detail.png');
    //alert($('.expand-button').children('.btn-open-close-plano').length);
    //IF THE NEXT SLIDE WASN'T OPEN THEN OPEN IT
    var expandContentElement = $(this).parent().parent().next().find(".plano-action-container");
    if (expandContentElement.is(':hidden') === true) {

        //ADD THE ON CLASS TO THE BUTTON
        expandContentElement.addClass('on');

        //alert($(this).children('.btn-open-close-plano').attr('src'));
        $(this).children('.btn-open-close-plano').attr('src', '/images/btn-open-planogram-detail.png');
        //OPEN THE SLIDE
        expandContentElement.slideDown('normal');
    }
    event.stopPropagation();
});

//handle showing the svg in a modal
$("body").on("click", 'a.view-version', function (event) {
    //retrieve the latest version of the svg
    var planogramId = $(this).data("planogramid");
    $.ajax({
        type: "GET",
        url: "/Api/PlanxApi/GetLatestVersion?planogramId=" + planogramId,
        //data: JSON.stringify(dupPlano),
        contentType: "application/json",
        error: function (result) {
            try {
                alert(result);
            }
            catch (e) {
                //var responseText = result.responseText;
                alert(result);
            }


        },
        success: function (message) {
            //put the svg into the modal
            var svg = decodeURIComponent(message);

            svg = svg.substring(1, svg.length);
            svg = svg.substring(0, (svg.length - 1));

            $('#VersionModal .modal-dialog .modal-body #svg-container').html(svg);
            var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
            var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
            var svgH = Number($('#VersionModal').find('svg')[0].getAttribute('height'));
            var svgW = Number($('#VersionModal').find('svg')[0].getAttribute('width'));
            var aspectWidth = ((w - 200) / svgH) * svgW;
            $('#VersionModal').find('svg')[0].setAttribute('viewbox', '0 0 ' + (h - 200) + ' ' + (w - 200))
            $('#VersionModal').find('svg')[0].setAttribute('height', h - 200);
            $('#VersionModal').find('svg')[0].setAttribute('width', w - 200);

            $('#VersionModal .modal-dialog')[0].setAttribute('width', (w - 200));
            if (svgH > svgW) {
                //this is a portrait layout - favour height over width
                if ((h - 200) > svgH) {
                    $('#VersionModal .modal-dialog')[0].style.height = (svgH) + 'px';
                    var aspectWidth = ((h - 200) / svgH) * svgW;
                    $('#VersionModal').find('svg')[0].setAttribute('viewbox', '0 0 ' + (h - 200) + ' ' + aspectWidth)
                    $('#VersionModal .modal-dialog')[0].style.width = (aspectWidth + 100) + 'px';
                    $('#VersionModal').find('svg')[0].setAttribute('width', aspectWidth);
                }
                else {
                    $('#VersionModal .modal-dialog')[0].style.height = (h - 200) + 'px';

                    //aspect ration calc for width
                    var aspectWidth = ((h - 200) / svgH) * svgW;
                    $('#VersionModal').find('svg')[0].setAttribute('viewbox', '0 0 ' + (h - 200) + ' ' + aspectWidth)
                    $('#VersionModal .modal-dialog')[0].style.width = (aspectWidth + 100) + 'px';
                    $('#VersionModal').find('svg')[0].setAttribute('width', aspectWidth);

                }

            }
            else {
                if ((w - 200) > svgW) {
                    $('#VersionModal .modal-dialog')[0].style.width = (svgW) + 'px';
                    //aspect ration calc for height
                    //var aspectHeight = (((w - 200) / svgH) * 0.1) * svgH
                    //$('#VersionModal .modal-dialog')[0].style.height = aspectHeight + 'px';
                }
                else {
                    $('#VersionModal .modal-dialog')[0].style.width = (w - 200) + 'px';
                }
            }
            if (svgW > w) { }

            //$('#VersionModal').attr('height')
            var modal = $('#VersionModal').modal();

        }
    });

});


//handle renaming the planogram
$('#planogram-tabs-container').on('click', 'a.btn-save-planogram', function () {
    event.preventDefault();
    //retrieve the latest version of the svg
    var planogramId = $(this).data("planogramid");
    var newName = currentEdit.name;

       // $(".planoName[planoid=" + planogramId + "]").text;

    //alert(newName);
    var renamePlano = { PlanogramId: planogramId, PlanogramName: newName };
    $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/RenamePlanogram",
        data: JSON.stringify(renamePlano),
        contentType: "application/json",
        error: function (message) {

        },
        success: function (message) {
            DisplayPlanograms(_currentTab);
            $(".planoName[planoid=" + currentEdit["planoID"] + "]").val(currentEdit["name"]);
            $(".planoName[planoid=" + currentEdit["planoID"] + "]").next(".plano-name-click").show()
            $(".planoName[planoid=" + currentEdit["planoID"] + "]").parent().siblings(".btn-save-plano-container").hide();
            $(".planoName[planoid=" + currentEdit["planoID"] + "]").prop("disabled", true).removeClass('pname-edit'); saveClicked = false;
            prevEdit["name"] = "";
            prevEdit["planoID"] = 0;
            currentEdit["planoID"] = 0;
            currentEdit["name"] = "";
        }
    });

});
//handle showing the preview jpeg in a modal
$('#planogram-tabs-container').on('click', 'a.view', function () {
    event.preventDefault();
    //retrieve the latest version of the svg
    var planogramId = $(this).data("planogramid");
    $.ajax({
        type: "GET",
        url: "/Api/PlanxApi/GetPlanogramPreview?planogramId=" + planogramId,
        //data: JSON.stringify(dupPlano),
        contentType: "application/json",
        error: function (result) {
            try {
                alert(result);
            }
            catch (e) {
                //var responseText = result.responseText;
                alert(result);
            }


        },
        success: function (message) {
            //put the svg into the modal
            var jpegURI = decodeURIComponent(message);
            //jpegURI = jpegURI.substring(1, jpegURI.length - 1);

            var windowArea = .8;
            var top = 100;

            var ratio = windowArea;

            var width = window.innerWidth * ratio;


            $('#LightBoxModal .modal-dialog .modal-body #jpeg-container').attr("src", jpegURI);
            $('#LightBoxModal .modal-dialog').attr("width", width);
            $('#LightBoxModal .modal-dialog .modal-body #jpeg-container').attr("width", width)
            $('#LightBoxModal').modal();

        }
    });
});

$('#planogram-tabs-container').on('click', '.btn-save-as-action', function () {
    $(".saveas-container").hide();
    var planoID = $(this).attr("planoid");
    // show new input box for save as
    $(".planoNewName[planoid=" + planoID + "]").addClass("pname-edit"); //change the background color to indicate edit mode
    $(".saveas-container[planoid=" + planoID + "]").show();
    //$(this).parent().next(".btn-save-plano-container").show();
    $(this).closest("tr").prev().find(".btn-save-as").click(function () {
        if ($(this).prev().find(".planoNewName").val() === $(this).parent().parent().find(".planoName").val()) {
            alert("Warning: You haven't changed the name");
            return false;
        } else {
            //var isUpdate = confirm("Is this an update?");
            $('#UpdateOrNewModal').data("planogramId", planoID);
            $('#UpdateOrNewModal').data("planoNewName", ($(this).prev().find(".planoNewName").val()));
            $('#UpdateOrNewModal').modal();

        }
        saveAsClicked = true;
    });
    // prevent button redirecting to new page
    return false;
});

$('#planogram-tabs-container').on('click', '.btn-delete-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/DeletePlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});


$('#planogram-tabs-container').on('click', '.btn-submit-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/SubmitPlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        },
        error: function (message) {
            alert("unable to submit planogram");
        }
    });
    
});

$('#planogram-tabs-container').on('click', '.btn-approve-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/ApprovePlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});

$('#planogram-tabs-container').on('click', '.btn-unapprove-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/UnApprovePlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});


$('#planogram-tabs-container').on('click', '.btn-validate-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/ValidatePlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});

$('#planogram-tabs-container').on('click', '.btn-unvalidate-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/ValidatePlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});

$('#planogram-tabs-container').on('click', '.btn-reject-planogram', function () {
    var planogramId = $(this).data("planogramid");
    return $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/RejectPlanogram?planogramId=" + planogramId,
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            //reload planogramsInProgress
            DisplayPlanograms(_currentTab);
        }
    });

});

//$('#planogram-tabs-container').on('click', '.btn-edit-planogram.btn-unlocked-planogram', function () {
//    var planogramId = $(this).data("planoid");
//    window.open('/home/your-planograms/edit-planogram?planogramId=' + planogramId);
//});


//$('#planogram-tabs-container').on('click', '.btn-locked-planogram', function (e) {
//    alert('this planogram is locked');
//    e.preventDefault(e);

//});


$('#planogram-tabs-container').on('click', '.btn-you-locked-planogram', function (e) {
    if (!confirm('this planogram is locked by you, editing this planogram may cause you to lose work from another session')) {
        e.preventDefault(e);
    }
});


$(function () {
    //alert(event.timeStamp);
    $('.btn-update-plano').click(async function (event) {
        event.stopPropagation();
        $(this).prop("disabled", true);
        $('.btn-new-build-plano').prop("disabled", true);
        var planoID = $('#UpdateOrNewModal').data("planogramId");
        var planoNewName = $('#UpdateOrNewModal').data("planoNewName");
        $('#preloader').toggle();
        await DuplicatePlanogram(true, planoID, planoNewName);
        DisplayPlanograms(_currentTab);
        $('#preloader').toggle();

    });
    $('.btn-new-build-plano').click(async function (event) {
        event.stopPropagation();
        $(this).prop("disabled", true);
        $('.btn-update-plano').prop("disabled", true);
        var planoID = $('#UpdateOrNewModal').data("planogramId");
        var planoNewName = $('#UpdateOrNewModal').data("planoNewName");
        $('#preloader').toggle();
        await DuplicatePlanogram(false, planoID, planoNewName);
        DisplayPlanograms(_currentTab);
        $('#preloader').toggle();

    });

});

$('#planogram-tabs-container').on('click', '.btn-cancel-save-as-planogram', function () {


    $(".saveas-container").hide();
    var planoID = $(this).attr("planoid");
    // show new input box for save as
    $(".planoNewName[planoid=" + planoID + "]").removeClass("pname-edit"); //change the background color to indicate edit mode
    //$(this).parent().next(".btn-save-plano-container").show();

    // prevent button redirecting to new page
    return false;
});



// on duplicate click 
function DuplicatePlanogram(isUpdate, planogramId, newPlanoName) {

    var dupPlano = { PlanogramId: planogramId, IsUpdate: isUpdate, NewPlanoName: newPlanoName };
    return $.ajax({
        type: "POST",
        url: "/Api/Planogram/DuplicatePlanogram",
        data: JSON.stringify(dupPlano),
        contentType: "application/json",
        success: function (message) {
            //alert(message);
            $('.btn-new-build-plano').prop("disabled", false);
            $('.btn-update-plano').prop("disabled", false);
            $('#UpdateOrNewModal').modal('toggle');
        }
    });
};


//function to add a new row
function showEditNameRow() {
    var rowCount = ($("#shade-list tr").length - 1); //remove the header row from the count
    var shadeName = "'shade[ " + rowCount + "].ShadeNumber'";
    var newRow = $("<tr><td></td><td><input id='" + rowCount + "_ProductId' name='" + rowCount + "_ProductId' type='hidden' value='0'/><input id='" + rowCount + "_ShadeNumber' name='" + rowCount + "_ShadeNumber' type='text'/></td><td><input id='" + rowCount + "_ShadeDescription' name='" + rowCount + "_ShadeDescription' type='text'/></td><td><input id=" + rowCount + "_Published' name" + rowCount + "_Published' type='checkbox'/></td></tr>");
    $("#shade-list").append(newRow)

}




///////////////////////////////////////////////////
// LOAD COUNTRIES BASED ON REGION SELECTION
///////////////////////////////////////////////////

$("#regionFilterList").change(function () {
    $("#hidRegion").val($(this).val());
    var regionId = $(this).val();
    populateCountryList(regionId);
});


$("#countriesFilterList").change(function () {
    $("#hidCountry").val($(this).val());
    var countryId = $(this).val();
    //populateRegionList(regionId);
});

$("#standTypeFilterList").change(function () {
    $("#hidStandType").val($(this).val());
    var countryId = $(this).val();
    //populateCountryList(regionId);
});



function populateCountryList(regionId) {

    var getRegionURL = "/api/settingsapi/getcountrylist?regionId=" + regionId; //+ "&callback=?";

    $.getJSON(getRegionURL, function (result) {
        var dd = $("#countriesFilterList");
        dd.empty();
        var option = new Option("Select a country", "");

        dd.append(option)
        $.each(result, function (index, optionData) {
            var option = new Option(optionData.text, optionData.value);
            dd.append(option);
        });
        if (result.length === 0) {
            option = new Option("No cassettes found", "0");
            $('#countriesFilterList').append(option);
        }
    });

}



function ConfirmDelete() {
    return confirm("Are you SURE you want to delete this item?");
};

/////////////////////////////////////////////////////////////////////////////////////////////
// Archive Planograms



$(function () {

    $('.btn-archive-plano').click(function (event) {
        var planoID = $('#ArchiveModal').data("planogramId");
        var jobCode = $('#ArchiveModal').data("job-code");
        var jobId = $('#ArchiveModal').data("jobid");
        ArchivePlanogram(planoID, jobCode, jobId);
        //BindPlanograms(); //located on the page;


    });
    $('.btn-choose-another-job').click(function (event) {
        $(this).closest('.modal').find('.modal-footer').hide();
        $(this).closest('.modal').find('.selected-job').hide();
        $(this).closest('.modal').find('#job-folder-accordion').show();

    });

});

//Archive Planogram Functions
/////////////////////////////////////////////////////
$('#planogram-tabs-container').on('click', '.btn-archive-planogram', function () {

    var planoID = $(this).data("planogramid");
    $('#ArchiveModal').data("planogramId", planoID);
    var getJobsURL = "/Api/YourPlanogramApi/GetJobFolders";
    var standTypeId = $('#standTypeFilterList').find(':selected').val() == "" ? 0 : parseInt($('#standTypeFilterList').find(':selected').val());
    var countryId = $('#countriesFilterList').find(':selected').val() == "" ? 0 : parseInt($('#countriesFilterList').find(':selected').val());
    var regionId = $('#regionFilterList').find(':selected').val() == "" ? 0 : parseInt($('#regionFilterList').find(':selected').val());


    //reusing the archiveplanoparams object
    var params = { JobCode: "", CountryId: countryId, RegionId: regionId, StandTypeId: standTypeId };
    //really we should be using the planogramId

    $.ajax({
        type: "POST",
        url: getJobsURL,
        data: JSON.stringify(params),
        contentType: "application/json",

        error: function (result) {
            try {
                var response = $.parseJSON(result.responseText);
                var errors = {};

                alert(response);
            }
            catch (e) {
                //var responseText = result.responseText;
                alert(result.responseText);
            }


        },
        success: function (result) {
            //insert rows into the table

            var modal = $('#ArchiveModal');
            var accordion = $('#job-folder-accordion');
            modal.find('.modal-body-title').text("Please select the job folder for this planogram.");
            accordion.show();
            accordion.empty();
            modal.find('.selected-job').hide();
            modal.find('.modal-footer').hide();

            if (result.length === 0) {
                // then there are no results
                modal.find('.modal-body-title').text("No Job Folders could be found for the selected country or region");
            }

            $.each(result, function (index, optionData) {
                accordion.append(
                    '<div class="panel panel-default"><div class="panel-heading modal-job-folder-container" role="tab" id="headingOne">' +
                    '<h4 class="panel-title">' +
                    '<a role="button" data-toggle="collapse" data-parent="#accordion" data-jobfolder-id="' + optionData["jobFolderId"] + '" href="#archive-jobfolder-' + optionData["jobFolderId"] + '"" aria-expanded="false" aria-controls="collapseOne">' +
                    optionData["name"] + '</a></h4></div></div>');

                var jobFolderPanel = '<div id="archive-jobfolder-' + optionData["jobFolderId"] + '" class="panel-collapse modal-collapse in job-number-container" role="tabpanel" aria-labelledby="headingOne">' +
                    '<div class="panel-body">';

                jobFolderPanel += ("</div></div>");
                accordion.append(jobFolderPanel);

            });

            // on Job Folder click
            $(".modal-job-folder-container").on('click', 'a', function (event) {

                var expanded = $(this).attr('aria-expanded');
                if (expanded === "false") {
                    var jobFolderId = $(this).data('jobfolder-id');
                    var target = $('#archive-jobfolder-' + jobFolderId);
                    GetJobNumbersForFolder(jobFolderId, target);
                }
                //event.stopPropagation();
            });
            $('.job-number-container').on('click', '.job-number-row', function (event) {
                //alert($(this).data("job-code"));
                $('#ArchiveModal').find('.modal-body-title').text("You've selected the following job.");
                var jobCode = $(this).data('job-number');
                var jobId = $(this).data('jobid');
                $('#ArchiveModal').data("job-code", jobCode);
                $('#ArchiveModal').data("jobid", jobId);

                //show selection, hide table
                $(this).closest('.modal').find('.selected-job .job-details').text(jobCode); // + $(this).data('customer-code') + ' ' + $(this).data('reason'));
                $(this).closest('#job-folder-accordion').hide();
                $(this).closest('.modal').find('.modal-footer').show();
                $(this).closest('.modal').find('.selected-job').show();

            });
            modal.addClass('centrescreen').removeClass('offscreen');

            modal.modal();
            //set up bootstrap collapse.
            $('.modal-collapse').collapse({
                toggle: true
            });

            modal.on('hidden.bs.modal', function (e) {
                // do something...
                modal.addClass('offscreen').removeClass('centrescreen');
            });
        }
    });

    // prevent button redirecting to new page
    return false;
});

// on Archive click
function ArchivePlanogram(planogramId, jobCode, jobId) {
    //alert('archiving planogram');
    var archivePlano = { PlanogramId: planogramId, JobNumber: jobCode, JobId: jobId };
    $.ajax({
        type: "POST",
        url: "/Api/YourPlanogramApi/ArchivePlanogram",
        data: JSON.stringify(archivePlano),
        contentType: "application/json",
        error: function (message) {

        },
        success: function (message) {
            
            $('#ArchiveModal').modal('hide');
            DisplayPlanograms(_currentTab);
        }
    });

}



//Planogram job functions
// on Job Folder click
$("body").on('click', '.jobfolder-panel a', function () {

    var expanded = $(this).attr('aria-expanded');
    if (expanded === "false") {
        var jobFolderId = $(this).data('jobfolder-id');
        var target = $('#jobfolder-' + jobFolderId);
        GetJobNumbersForFolder(jobFolderId, target);
    }
});

function GetJobNumbersForFolder(jobFolderId, target) {

    var getJobNoUrl = "/Api/YourPlanogramApi/GetJobNumbersForFolder?jobFolderId=" + jobFolderId;
    $.ajax({
        type: "GET",
        url: getJobNoUrl,
        //data: JSON.stringify(params),
        contentType: "application/json",

        error: function (result) {
            try {
                var response = $.parseJSON(result.responseText);
                var errors = {};

                alert("unable to retrieve the jobNumbers");
                console.log(response.MessageDetail);
            }
            catch (e) {
                //var responseText = result.responseText;
                alert(result.responseText);
            }


        },
        success: function (result) {
            target.find('.panel-body').empty();

            if (result.length !== 0) {

                $.each(result, function (index, jobData) {
                    jobRow = (
                        '<div class="job-number-row"' + 'data-job-number="' + jobData["jobCode"] + '" data-customer-code="' + jobData["customerCode"] + '" data-jobid="' + jobData["jobId"] + '" data-reason="' + jobData["reason"] + '">' +
                        '<div class="acc-jobnumber-field"> Job Number: ' + jobData["jobCode"] + '</div>' +
                        '</div>'
                    );

                    target.find('.panel-body').append(jobRow);

                });
            }
        }
    });
}

// on Job Code click

$('body').on('click', '.job-number-row', function (event) {

    var jobCode = $(this).data('job-number').trim();
    var jobId = $(this).data('jobid');
    DisplayArchivedPlanograms((jobCode), jobId, 7);

});

$('#planogram-tabs-container').on('click', 'button#view-folders', function () {
    $('.job-folder-container').show();
    $('.plano-results').hide();
    var container = $('#planogram-tabs-container').find('#job-number-results');
    container.empty();
});

//obsolete
function GetSkuDownload(planogramId) {
    var getSkuListFileURL = "/api/YourPlanogramApi/getSkuList?planogramId=" + planogramId; //+ "&callback=?";


    $.get(getSkuListFileURL)
        .done(function (data) {
            //alert(data)
            var decodedData = decodeURIComponent(data);
            var fileUrl = decodedData.replaceAll('"', '');
            window.location.href = fileUrl;
        })
        .fail(function () { alert('failed'); })
}

function GetJsonSkuDownload(planogramId, planogramName) {
    var getSkuListFileURL = "/api/YourPlanogramApi/getJsonSkuList?planogramId=" + planogramId; //+ "&callback=?";


    $.get(getSkuListFileURL)
        .done(function (data) {
            var ddata = data.replace(/\+/g, ' ');
            var rows = JSON.parse(decodeURIComponent(ddata));
            //support diam efacs hardcoded import structure
            //rows.forEach(obj => renameKeys(obj));

            const worksheet = XLSX.utils.json_to_sheet(rows);
            const workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, "SKUList");
            XLSX.writeFile(workbook, planogramName + "-SkuList.xlsx", { compression: true });
        })
        .fail(function (response) { alert('failed'); })
}
//functions to support diam efacs hardcoded import structure
function renameKeys(row) {
   var skuListKeyArray = [
        "ColumnId",
        "ColumnPosition",
        "Category",
        "SubCategory",
        "partNumber",
        "altPartNumber",
        "customerRefNo",
        "partName",
        "partStatus",
        "productName",
        "shadeName",
        "facingStatus",
        "shadeEAN",
        "facings",
        "stock",
        "totalSKU",
        "width",
        "height",
        "unitCost"
]

        skuListKeyArray.forEach(key => {
            newkey = key.charAt(0).toUpperCase() + key.slice(1);
            renameKey(row, key, newkey)
        });
    }
function renameKey(row, oldKey, newKey) {
    row[newKey] = row[oldKey];
    delete row[oldKey]
}
function GetSkuFile(planogramId) {
    //var planogramId = $(this).data("planoid");
    var getSkuListFileURL = "/api/YourPlanogramApi/getSkuList?planogramId=" + planogramId; //+ "&callback=?";

    $.ajax({
        type: "GET",
        url: getSkuListFileURL,
        //data: JSON.stringify(params),
        contentType: "application/json",

        error: function (result) {
            try {
                var response = $.parseJSON(result.responseText);
                var errors = {};

                alert("unable to retrieve the file");
                console.log(response.MessageDetail);
            }
            catch (e) {
                //var responseText = result.responseText;
                alert(result.responseText);
            }


        },
        success: function (result) {
            window.location.href = result.path;
        }
    });
};

$("yp-tabs").tab;
$('#yp-tabs a:first').tab('show');
DisplayPlanograms(1);
//Manage Tabs
$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    e.target // newly activated tab
    e.relatedTarget // previous active tab
    var target = e.target.id;
    switch (target) {
        case "tab-inprogress":
            DisplayPlanograms(1);
            _currentTab = 1;
            break;
        case "tab-submitted":
            DisplayPlanograms(2);
            _currentTab = 2;
            break;
        case "tab-approved":
            DisplayPlanograms(5);
            _currentTab = 5;
            break;
        case "tab-archived":
            DisplayArchiveFolders(7);
            _currentTab = 7;
            break;
        default:
            _currentTab = 1;
            DisplayPlanograms(1);
    }
})

$('#requestsFilterPanel').on('click', '#btnFilter', function () {
    if (_currentTab == 7) {
        DisplayArchiveFolders(7);
    }
    else {
        DisplayPlanograms(_currentTab);
    }
    var countryId = $(this).val();
    //populateCountryList(regionId);
});
function DisplayPlanograms(statusId) {

    if (statusId == 7) {  //archived

        DisplayArchiveFolders(statusId);
        return;
    }


    var containerStart = '<div class="planogram" id="planogramRow" runat="server"><table width="100%" cellspacing="0" cellpadding="2" border="0">' +
        '<tbody><tr><td class="container-row">';
    var containerEnd = '</td> <!-- end of container row --></tr></tbody></table></div>';

    var getPlanosURL = "/Api/YourPlanogramApi/GetPlanograms";
    var getRoleUrl = "/Api/AuthApi/GetUserRole";
    var standTypeId = $('#standTypeFilterList').find(':selected').val() != "" ? $('#standTypeFilterList').find(':selected').val() : 0;
    var countryId = 0;
    if ($('#countriesFilterList').find(':selected').val() != undefined) {
        countryId = $('#countriesFilterList').find(':selected').val() != "" ? $('#countriesFilterList').find(':selected').val() : 0;
    }
    var regionId = 0;
    if ($('#regionFilterList').find(':selected').val() != undefined) {
        regionId = $('#regionFilterList').find(':selected').val() != "" ? $('#regionFilterList').find(':selected').val() : 0;
    }

    var IPPlanoParams = { status: statusId, JobCode: "", CountryId: countryId, RegionId: regionId, StandTypeId: standTypeId };
    $('#preloader').toggle();
    $.ajax({
        type: "POST",
        url: getPlanosURL,
        data: JSON.stringify(IPPlanoParams),
        contentType: "application/json",
        error: function (message) {
            alert("could not load planogram");
        },
        success: function (result) {
            $.getJSON(getRoleUrl, function (role) {
                var tabId = "#tab-" + statusId;

                var userRole = role.role;
                var container = $('#planogram-tabs-container').find(tabId);
                container.empty();

                if (result.length === 0) {
                    container.empty();
                    var noResults = "<h4>No results found with the current filter</h4>";
                    container.append(noResults);
                }



                $.each(result, function (index, plano) {
                    var IsValidated = "";
                    var disableSubmit = "";
                    var planogramLocked = "";

                    if (plano.statusId == 6) //validated
                        IsValidated = " validated";

                    var planogramTable = '<table class="planogram-table"><tbody><tr>';
                    if (plano.locked) {
                        disableSubmit = "disabled"
                        planogramLocked = '<td class="is-locked-icon"><span id="spnLocked" >&#x1F512; </span></td>';
                        planogramTable = planogramTable;
                    }
                    var expandDetail = '<td class="expand-detail"><a class="expand-button" href="javascript:void(0);">' +
                        '<img class="btn-open-close-plano" src="/images/btn-closed-planogram-detail.png" alt="show detail"></a></td>';
                    var hasComments = '';
                    var tableNameCell = '<td class="planogram-tbl-cell planogram-name-cell">' +
                        '<div class="rename-container clearfix">' +
                        '<div class="planogram-name' + IsValidated + '"><input name="" value="' +
                        plano.name + '" class="planoName" planoid="' + plano.id + '">' +
                        '<div class="plano-name-click" style="position:absolute; left:0; right:0; top:0; bottom:0;"></div></div>';
                    var btnSavePlanoContainer = '<div class="btn-save-plano-container clearfix" style="display: none;">' +
                        '<a href="javascript:void(0);" id="btnRenamePlanogram"  class="butn planogram-buttons btn-save-planogram" type="button" style="display:block;" data-planogramid="' + plano.id + '">Save</a>' +
                        '</div >'
                    var planoInfo = '<div class="plano-modified-date">' + plano.formattedDateUpdated + '<br>by: ' + plano.lubName + '</div>';

                    var saveAs = '<div class="saveas-container clearfix" planoid="' + plano.id + '">' +
                        '<div class="planogram-newname" planoid="' + plano.id + '">' +
                        '<input name="" type="text" value="' + plano.name + '" id="" class="planoNewName" planoid="' + plano.id + '">' +
                        '<div class="plano-name-click" style="position:absolute; left:0; right:0; top:0; bottom:0;"></div >' +
                        '</div>' +
                        '<button type="button" id="btnJSSaveAs" class="btn-save-as planogram-buttons" data-planogramid="' + plano.id + '" data-planogramtitle="' + plano.name + '">Save As</button>' +
                        '<input type="button" id="btnCancelSaveAs" value="Cancel" planoid="' + plano.id + '" class="btn-cancel-save-as-planogram planogram-buttons">' +
                        '</div> ' +
                        '</td></tr>';

                    var buttonRow = '<tr class="button-row">' +
                        '<td class="planogram-tbl-cell planogram-tbl-action" colspan="3">' +
                        '<div class="plano-action-container accordion-content" style="display:none">' +
                        '<div class="button-row row">';

                    var viewBtn = '';
                    var editBtn = '';
                    var saveAsBtn = '';
                    var submitBtn = '';
                    var deleteBtn = '';
                    var rejectBtn = '';
                    var approveBtn = '';
                    var excelBtn = '';
                    var commentBtn = '';
                    var validateBtn = '';
                    var orderBtn = '';
                    var archiveBtn = '';

                    if (plano.hasVersion) {
                        viewBtn = '<a href="javascript:void(0);" class="butn planogram-buttons view-version" type="button" style="display:block;" data-planogramid="' + plano.id + '">View</a>';
                    }
                    else if (plano.planogramPreviewSrc !== null && typeof (plano.planogramPreviewSrc) !== 'undefined') {
                        //if (plano.planogramPreviewSrc.substring(0, 4) == 'data') {
                        viewBtn = '<a href="javascript:void(0);" id="btnPlanoPreview" data-planogramId="' + plano.id + '" target="_blank" type="button" class="butn planogram-buttons view">View</a>';
                        //}
                    }

                    editBtn = '<a href="javascript:void(0);" id = "btnPlanxEdit" class="butn planogram-buttons planx-edit" type="button"  data-planogramid="' + plano.id + '" data-planogramtitle="' + plano.name + '" >Edit</a>';

                    deleteBtn = '<a href="javascript:void(0);" name="btnDeletePlanogram" class="butn btn-delete-planogram planogram-buttons" data-planogramid="' + plano.id + '" >Delete</a>';

                    btnSaveAs = '<button type="button" planoid="' + plano.id + '" class="butn btn-save-as-action planogram-buttons">Save As</button>';
                    excelBtn = '<button type="button" name="" id=skulist-"' + plano.id + '" class="butn btn-export-sku-list planogram-buttons" onclick="GetJsonSkuDownload(' + plano.id + ', \'' + plano.name + '\')" data-planoid="' + plano.id + '">Excel Planogram</button>';

                    let commentClass = 'has-comment';
                    let commentCount = plano.commentCount;
                    if (plano.commentCount == 0) {
                        commentClass = 'no-comment'
                        commentCount = ''
                    }
                    commentBtn = '<button type="button" class="butn planogram-buttons view-notes" data-planogramid="' + plano.id + '" data-planogramtitle="' + plano.name + '">Comments</button>' +
                        '<span id="spnHasComments" class="' + commentClass + '">' + commentCount + '</span>';

                    switch (statusId) {
                        case 1: //inrprogress
                            submitBtn = '<a href="javascript:void(0);" id=btnSubmitPlanogram" ' + disableSubmit + ' value="Submit" name="btnSubmitPlanogram" class="butn btn-submit-planogram planogram-buttons';
                            if (plano.locked) {
                                submitBtn = submitBtn + ' disable-click '
                            }
                            submitBtn = submitBtn + '" data-planogramid="' + plano.id + '">Submit</a>';
                            break;
                        case 2: //submitted
                            approveBtn = '<a href="javascript:void(0);" id=btnApprovePlanogram" value="Approve" name="btnApprovePlanogram" class="butn btn-approve-planogram planogram-buttons"  data-planogramid="' + plano.id + '">Approve</a>';
                            rejectBtn = '<a href="javascript:void(0);" id=btnRejectPlanogram" value="Reject" name="btnRejectPlanogram" class="butn btn-reject-planogram planogram-buttons"  data-planogramid="' + plano.id + '">Reject</a>';
                            archiveBtn = '<a href="javascript:void(0);" id=btnArchivePlanogram" value="Archive" name="btnArchivePlanogram" class="butn btn-archive-planogram planogram-buttons"  data-planogramid="' + plano.id + '">Archive</a>';
                            break;
                        case 5: //approved
                            var valLable = "Validate";
                            if (plano.statusId == 6) //Validated
                            {
                                valLable = "UnValidate"
                            }
                            validateBtn = '<a href="javascript:void(0);" id=btnValidatePlanogram" value="Archive" name="btnValidatePlanogram" class="butn btn-validate-planogram planogram-buttons"  data-planogramid="' + plano.id + '">' + valLable + '</a>';
                            //buttonRow = buttonRow + '<a href="javascript:void(0);" id=btnUnValidatePlanogram" value="Archive" name="btnUnValidatePlanogram" class="butn btn-unvalidate-planogram planogram-buttons"  data-planogramid="' + plano.id + '"></a>';
                            archiveBtn = '<a href="javascript:void(0);" id=btnArchivePlanogram" value="Archive" name="btnArchivePlanogram" class="butn btn-archive-planogram planogram-buttons"  data-planogramid="' + plano.id + '">Archive</a>';
                            if (role.shopper) {
                                orderBtn = '<a href="javascript:void(0);" id=btnAddToOrder" value="Archive" name="btnAddToOrder" class="butn btn-order-planogram planogram-buttons"  data-planogramid="' + plano.id + '">Add To Order</a>';
                            }
                            break;

                    }

                    switch (userRole) {
                        case "administrator":
                            switch (statusId) {
                                case 1:
                                    buttonRow = buttonRow + viewBtn + editBtn + btnSaveAs + excelBtn + submitBtn + deleteBtn
                                    break;
                                case 2:
                                    buttonRow = buttonRow + viewBtn + editBtn + btnSaveAs + excelBtn + archiveBtn + approveBtn + rejectBtn
                                    break;
                                case 5:
                                    buttonRow = buttonRow + viewBtn + btnSaveAs + validateBtn + archiveBtn + excelBtn + orderBtn                                  
                                    break;
                                default:
                                    buttonRow = buttonRow + viewBtn + editBtn + btnSaveAs + excelBtn + archiveBtn
                                    break;
                            }
                            break;
                        case "approver":
                            switch (statusId) {
                                case 1:
                                    buttonRow = buttonRow + viewBtn + editBtn + excelBtn + submitBtn
                                    break;
                                case 2:
                                    buttonRow = buttonRow + viewBtn + excelBtn + approveBtn
                                    break;
                                case 5:
                                    buttonRow = buttonRow + viewBtn
                                    if (role.creator) {
                                        buttonRow = buttonRow + btnSaveAs
                                    }
                                    if (role.archiver) {
                                        buttonRow = buttonRow + archiveBtn
                                    }
                                    buttonRow = buttonRow + excelBtn

                                    break;
                                default:
                                    buttonRow = buttonRow + viewBtn + btnSaveAs + excelBtn
                                    break;
                            }
                            break;
                        case "validator":
                            switch (statusId) {
                                case 1:
                                    buttonRow = buttonRow + viewBtn + editBtn + excelBtn + submitBtn
                                    break;
                                case 2:
                                    buttonRow = buttonRow + viewBtn
                                    if (role.creator) {
                                        buttonRow = buttonRow + btnSaveAs
                                    }
                                    buttonRow = buttonRow + excelBtn
                                    if (role.archiver) {
                                        buttonRow = buttonRow + archiveBtn
                                    }
                                    buttonRow = buttonRow + approveBtn
                                    break;
                                case 5:
                                    buttonRow = buttonRow + viewBtn
                                    if (role.creator) {
                                        buttonRow = buttonRow + btnSaveAs
                                    }
                                    if (role.archiver) {
                                        buttonRow = buttonRow + archiveBtn
                                    }
                                    buttonRow = buttonRow + excelBtn
                                    break;
                                default:
                                    buttonRow = buttonRow + viewBtn + excelBtn
                                    break;
                            }
                            break;
                        //case "clientEditor":
                        //    switch (statusId) {
                        //        case 1:
                        //            buttonRow = buttonRow + viewBtn + editBtn + excelBtn + submitBtn
                        //            break;
                        //        default:
                        //            buttonRow = buttonRow + viewBtn + excelBtn
                        //            break;
                        //    }
                        //    break
                        default:
                            switch (statusId) {
                                case 1:
                                    buttonRow = buttonRow + viewBtn + editBtn + excelBtn + submitBtn
                                    break;
                                default:
                                    buttonRow = buttonRow + viewBtn + excelBtn
                                    break;
                            }
                            break;

                    }
                    if (role.shopper) {
                        buttonRow = buttonRow + orderBtn + commentBtn
                    }
                    else {
                        buttonRow = buttonRow + commentBtn
                    }

                    buttonRow = buttonRow + '</div>';
                    endButtonDivRow = '</div>';

                    var detailRow = '<div class="detail-row row"><ul>' +
                        '<li><div>' + plano.standName + ' | ' + plano.standWidth + ' x ' + plano.standHeight + '</div></li>' +
                        '<li><div>' + plano.shelfCount + ' Shelves | ' + plano.accessoryCount + ' Accessories</div></li>' +
                        '</ul></div>';

                    var lockedRow = "</div>";
                    if (plano.locked) {
                        let lockDate = new Date(plano.dateLocked);
                        let month = lockDate.toLocaleString('default', { month: 'short' });
                        lockedRow = '<div class="row plano-locked-detail">CURRENTLY LOCKED: ' + lockDate.getDate() + ' ' + month + ' ' + lockDate.getFullYear() + ' BY ' + plano.lockingUserName + '</div></div>'
                    }


                    var endofButtonRow = '</td></tr> <!-- end of button row -->';
                    var endofTable = '</tbody></table>';
                    var planoRow = planogramTable + expandDetail + planogramLocked + hasComments + tableNameCell + btnSavePlanoContainer + planoInfo;
                    if (userRole !== "clientEditor") {
                        planoRow = planoRow + saveAs;
                    }
                    planoRow = planoRow + buttonRow + detailRow + lockedRow + endofButtonRow + endofTable;
                    container.append(containerStart + planoRow + containerEnd);
                });
            }); //end of getrole
            $('#preloader').toggle();
        }
    });
}



function DisplayArchiveFolders(statusId) {


    var getJobsURL = "/Api/YourPlanogramApi/GetJobFolders";
    var getRoleUrl = "/Api/AuthApi/GetUserRole";
    var standTypeId = $('#standTypeFilterList').find(':selected').val() != "" ? $('#standTypeFilterList').find(':selected').val() : 0;
    var countryId = 0;
    if ($('#countriesFilterList').find(':selected').val() != undefined) {
        countryId = $('#countriesFilterList').find(':selected').val() != "" ? $('#countriesFilterList').find(':selected').val() : 0;
    }
    var regionId = 0;
    if ($('#regionFilterList').find(':selected').val() != undefined) {
        regionId = $('#regionFilterList').find(':selected').val() != "" ? $('#regionFilterList').find(':selected').val() : 0;
    }
    
    var params = { status: statusId, JobCode: "", CountryId: countryId, RegionId: regionId, StandTypeId: standTypeId, ExcludeEmptyFolders: 1 };
    $('#preloader').toggle();

    $.ajax({
        type: "POST",
        url: getJobsURL,
        data: JSON.stringify(params),
        contentType: "application/json",
        success: function (result) {
            try {
                $.getJSON(getRoleUrl, function (role) {

                    var tabId = "#tab-" + statusId;
                    var userRole = role.role;
                    var container = $('#planogram-tabs-container').find(tabId);
                    container.empty();
                    var jobFoldersContainer = '<div class="job-folder-container"><div class="panel-group" id="archived-job-folders" role="tablist" aria-multiselectable="true">';
                    var resultsContainers = '<div class="planos-container"><div class="approved-list  plano-results"><div id="job-number-results"></div>' +
                        '<div id="job-number-results-buttons"><button type="button" id="view-folders" class="planogram-buttons">Show Folders</button></div></div></div>';
                    //var result = $.parseJSON(result);
                    var errors = {};
                    var jobfolderList = '';
                    $.each(result, function (index, jobFolder) {
                        jobfolderList = jobfolderList + '<div class="panel panel-maxfactor jobfolder-panel">';
                        jobfolderList = jobfolderList + '<div class="panel-heading" role = "tab" id = "heading-' + jobFolder.jobFolderId + '" >';
                        jobfolderList = jobfolderList + '<h4 class="panel-title">';
                        jobfolderList = jobfolderList + '<a role="button" data-toggle="collapse" data-parent="#accordion" data-jobfolder-id="' + jobFolder.jobFolderId + '" href="#jobfolder-' + jobFolder.jobFolderId + '" aria-expanded="false" aria-controls="collapseOne" class="collapsed">';
                        jobfolderList = jobfolderList + jobFolder.name;
                        jobfolderList = jobfolderList + '</a></h4></div></div>';
                        jobfolderList = jobfolderList + '<div id="jobfolder-' + jobFolder.jobFolderId + '" class="panel-collapse job-number-container collapse" role = "tabpanel" aria - labelledby="heading-' + jobFolder.jobFolderId + '" aria - expanded="false" style = "height: 0px;" >';
                        jobfolderList = jobfolderList + '<div class="panel-body">';
                        jobfolderList = jobfolderList + '</div></div>';

                    });

                    jobFoldersContainer = resultsContainers + jobFoldersContainer + jobfolderList + '</div></div>';
                    container.append(jobFoldersContainer);

                });
                $('#preloader').toggle();

            }
            catch (e) {
                    //var responseText = result.responseText;
                    alert(result.responseText);
            }
            //$('#preloader').toggle();

            },
            error: function (message) {
                alert("Error displaying archive");
                $('#preloader').toggle();

             }

    });
}

function DisplayArchivedPlanograms(jobCode, jobId, statusId) {



    var containerStart = '<div class="planogram" id="planogramRow" runat="server"><table width="100%" cellspacing="0" cellpadding="2" border="0">' +
        '<tbody><tr><td class="container-row">';
    var containerEnd = '</td> <!-- end of container row --></tr></tbody></table></div>';

    var getPlanosURL = "/Api/YourPlanogramApi/GetArchivedPlanogramsByJob";
    var getRoleUrl = "/Api/AuthApi/GetUserRole";
    var standTypeId = $('#standTypeFilterList').find(':selected').val() != "" ? parseInt($('#standTypeFilterList').find(':selected').val()) : 0;
    var countryId = 0;
    if ($('#countriesFilterList').find(':selected').val() != undefined) {
        countryId = $('#countriesFilterList').find(':selected').val() != "" ? $('#countriesFilterList').find(':selected').val() : 0;
    }
    var regionId = 0;
    if ($('#regionFilterList').find(':selected').val() != undefined) {
        regionId = $('#regionFilterList').find(':selected').val() != "" ? $('#regionFilterList').find(':selected').val() : 0;
    }
    
    var archivedPlanoParams = { Status: 0, JobId: jobId, JobCode: jobCode, CountryId: countryId, RegionId: regionId, StandTypeId: standTypeId };

    $.ajax({
        type: "POST",
        url: getPlanosURL,
        data: JSON.stringify(archivedPlanoParams),
        contentType: "application/json",
        error: function (message) {
            alert("Error loading planograms");
        },
        success: function (result) {
            $.getJSON(getRoleUrl, function (role) {
                var tabId = "#tab-" + statusId;

                var userRole = role.role;
                var container = $('#planogram-tabs-container').find(tabId).find('#job-number-results');
                container.empty();

                if (result.length === 0) {
                    container.empty();
                    var noResults = "<h4>No results found for job number <span style='textTransform:upper;'>" + jobCode + "</span> with the current filter</h4>";
                    container.append(noResults);
                } else {
                    var hasResults = "<h4>" + result.length + " planograms found for job number <span style='textTransform:upper;'>" + jobCode + "</span> with the current filter</h4>";
                    container.append(hasResults);
                }

                $.each(result, function (index, plano) {
                    var planogramTable = '<table class="planogram-table"><tbody><tr>';
                    var expandDetail = '<td class="expand-detail"><a class="expand-button" href="javascript:void(0);">' +
                        '<img class="btn-open-close-plano" src="/images/btn-closed-planogram-detail.png" alt="show detail"></a></td>';
                    var hasComments = '';
                    var tableNameCell = '<td class="planogram-tbl-cell planogram-name-cell">' +
                        '<div class="rename-container clearfix">' +
                        '<div class="planogram-name"><input name="" value="' +
                        plano.name + '" class="planoName" planoid="' + plano.id + '">' +
                        '<div class="plano-name-click" style="position:absolute; left:0; right:0; top:0; bottom:0;"></div></div>';

                    var planoInfo = '<div class="plano-modified-date">' + plano.formattedDateUpdated + '<br>by: ' + plano.lubName + '</div>';

                    var saveAs = '<div class="saveas-container clearfix" planoid="' + plano.id + '">' +
                        '<div class="planogram-newname" planoid="' + plano.id + '">' +
                        '<input name="" type="text" value="' + plano.name + '" id="" class="planoNewName" planoid="' + plano.id + '">' +
                        '</div>' +
                        '<button type="button" id="btnJSSaveAs" class="btn-save-as planogram-buttons" data-planogramid="' + plano.id + '" data-planogramtitle="' + plano.name + '">Save As</button>' +
                        '<input type="button" id="btnCancelSaveAs" value="Cancel" planoid="' + plano.id + '" class="btn-cancel-save-as-planogram planogram-buttons"></div>' +
                        '</td></tr>';

                    var buttonRow = '<tr class="button-row">' +
                        '<td class="planogram-tbl-cell planogram-tbl-action" colspan="3">' +
                        '<div class="plano-action-container accordion-content" style="display:none">' +
                        '<div class="button-row row">';
                    if (plano.hasVersion) {
                        buttonRow = buttonRow + '<a href="javascript:void(0);" class="butn planogram-buttons view-version" type="button" style="display:block;" data-planogramid="' + plano.id + '">View</a>';
                    }
                    else if (plano.planogramPreviewSrc !== null && typeof (plano.planogramPreviewSrc) !== 'undefined') {
                        //if (plano.planogramPreviewSrc.substring(0, 4) == 'data') {
                        buttonRow = buttonRow + '<a href="javascript:void(0);" id="btnPlanoPreview" data-planogramId="' + plano.id + '" target="_blank" type="button" class="butn planogram-buttons view">View</a>';
                        //}
                    }

                    if (userRole !== "clientEditor") {
                        buttonRow = buttonRow + '<button type="button" planoid="' + plano.id + '" class="butn btn-save-as-action planogram-buttons">Save As</button>';
                    }
                    buttonRow = buttonRow + '<button type="button" name="" id=skulist-"' + plano.id + '" class="butn btn-export-sku-list planogram-buttons" onclick="GetJsonSkuDownload(' + plano.id + ', \'' + plano.name + '\')" data-planoid="' + plano.id + '">Excel Planogram</button>';

                    buttonRow = buttonRow + '<button type="button" class="butn planogram-buttons view-notes" data-planogramid="' + plano.id + '" data-planogramtitle="' + plano.name + '">Comments</button>' +
                        '<span id="spnHasComments" class="has-comment">' + plano.commentCount + '</span>' +
                        '</div>';

                    var detailRow = '<div class="detail-row row"><ul>' +
                        '<li><div>' + plano.standName + ' | ' + plano.standWidth + ' x ' + plano.standHeight + '</div></li>' +
                        '<li><div>' + plano.shelfCount + ' Shelves | ' + plano.accessoryCount + ' Accessories</div></li>' +
                        '</ul></div></div>';

                    var endofButtonRow = '</td></tr> <!-- end of button row -->';
                    var endofTable = '</tbody></table>';
                    var planoRow = planogramTable + expandDetail + hasComments + tableNameCell + planoInfo;
                    if (userRole !== "clientEditor") {
                        planoRow = planoRow + saveAs;
                    }
                    planoRow = planoRow + buttonRow + detailRow + endofButtonRow + endofTable;
                    container.append(containerStart + planoRow + containerEnd);
                });

                $('.job-folder-container').hide();
                $('.plano-results').show();

            }); //end of getrole
        }
    });

}


    ////////////////////////////////// ORDERS SECTION ///////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////


        // handle stand type check behaviourtn-order
        $('#planogram-tabs-container').on("click", ".btn-order-planogram", function (event) {
            event.preventDefault();
            //alert('hello');
            var planogramId = $(this).data("planogramid");
            var planogramTitle = $(this).data("planogramtitle");

            getOrders(planogramId, planogramTitle);



        });


function getOrders(planogramID, planogramTitle) {
    var getOrdersURL = "/Api/YourPlanogramApi/GetOpenOrders?planogramId=" + planogramID; // + "&callback=?";

    $.ajax({
        type: "GET",
        url: getOrdersURL,
        //data: JSON.stringify(params),
        contentType: "application/json",

        error: function (result) {
            try {
                $('#orderModal').find('.alert').removeClass('alert-success').addClass('alert-danger').show();
                $('#orderModal').find('#alert-title').text('Error');
                $('#orderModal').find('#alert-message').text(result.responseText);
                $('#orderModal').find('.order-details').hide();
                $('#orderModal').find('.btn-add-order').hide();
                $('#orderModal').modal();
            }
            catch (e) {
                alert(result.responseText);
            }

        },
        success: function (result) {
            $('#orderModal').find('.alert').removeClass('alert-danger').hide();

            $('#orderModal').data("planogramId", planogramID);
            $('#orderModal').data("planogramTitle", planogramTitle);
            $('#orderModal').find('.btn-add-order').show();
            $('#orderModal').find('.order-details').show();

            $('#select-order').find('option').remove();

            $.each(result, function (i, obj) {
                $('#select-order').append($('<option>', {
                    value: obj.orderId,
                    text: obj.orderTitle
                }));
            });

            $('#orderModal').modal();

        }
    });
}

$(function () {
    //alert(event.timeStamp);
    $('.btn-add-order').click(function (event) {

        $(this).hide();
        $('#orderModal').find('.btn-cancel').hide();

        var quantity = $('#orderModal').find('#orderquantity').val();
        var orderId = $('#orderModal').find('#select-order').val();
        var planogramId = $('#orderModal').data("planogramId");

        if (isNaN(quantity) || quantity < 1) {
            $('#orderModal').find('.alert').removeClass('alert-success').addClass('alert-danger').show();
            $('#orderModal').find('#alert-title').text('Error');
            $('#orderModal').find('#alert-message').text('Please enter a numeric quantity of at least 1');

            $(this).show();
            $('#orderModal').find('.btn-cancel').show();
            return;
        };

        var isFullPlano = $('input[name=isFullPlano]:checked').val();


        var newOrder = { orderId: orderId, planogramId: planogramId, quantity: quantity, isFullPlano: isFullPlano };

        $.ajax({
            type: "POST",
            url: "/Api/YourPlanogramApi/AddToOrder",
            //data: 'act=add-com&name='+theName.val()+'&email='+theMail.val()+'&comment='+theCom.val(),
            data: JSON.stringify(newOrder),
            contentType: "application/json",
            success: function (html) {
                //say Thankyou
                $('#orderModal').find('.alert').removeClass('alert-danger').hide();
                var isFullPlano = $('input[name=isFullPlano]:checked').val();
                if (isFullPlano === 'true') {
                    alert('Thank you, full unit has been added to your order');
                } else {
                    alert('Thank you, updates have been added to your order');
                }
                //hide the modal
                $('#orderModal').modal('hide');
            },
            error: function (result) {

                $('#orderModal').find('#alert-title').html('Error');
                $('#orderModal').find('#alert-message').html(result.responseJSON.ExceptionMessage);
                $('#orderModal').find('.order-details').hide();
                $('#orderModal').find('.alert').show();
                $(this).show();
                $('#orderModal').find('.btn-cancel').show();

            }
        });
    });

});




