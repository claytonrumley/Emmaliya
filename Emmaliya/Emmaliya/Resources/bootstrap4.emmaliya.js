/************************************************
    alert
    Generates a popup and display the passed message
************************************************/
emmaliya.fn.alert = function (message, options) {
    var popup = $('<div><div class="emma-popup" title="' + emmaliya.currentApplication.name + '" data-emma-buttons="[{Text:\'OK\', Action:\'Cancel\'}]"><div><p class="emma-popup-message"></p><p class="emma-popup-stack-trace"></p></div></div></div>');

    $('body').append(popup);

    //modify message based on passed value  
    var textToShow = message;

    if (message.msg) textToShow = message.msg;

    popup.find('.emma-popup-message').html(textToShow);

    //Go through the options and retrieve additional information to be displayed.
    if (options !== undefined) {
        popup.find('.emma-popup').attr("title", options.title);
        popup.find('.emma-popup').attr("data-emma-buttons", options.buttons);

        if (options.toggle == "Yes") {
            popup.find('.emma-popup-stack-trace').html(message.stktrc);
        }
    };

    emmaliya.initialize(popup);

    popup.find('.emma-popup').modal('show');
};

/************************************************
    confirm
    Generates a popup and display the with given message
    and options to click on
************************************************/
emmaliya.fn.confirm = function (message, options, onOk, onCancel) {
    var opt = {
        title: emmaliya.currentApplication.name, //The text to show in the popup header
        closeButtonIndex: -1, //The button index that the close button (the 'x' in the top-right) maps to
        icon: 'fas fa-question-circle text-warning', //The icon to display on the left side of the popup
        //The list of buttons for the user to click on
        buttons: [{ text: 'OK', style: 'btn-success', click: onOk }, { text: 'Cancel', style: 'btn-warning', click: onCancel }]
    };

    //Merge options into opt (opt's values will be replaced by options if there are duplicates,
    //otherwise they'll be left alone):
    //Object.assign(opt, options);
    jQuery.extend(opt, options);

    var modal = $('<div class="modal fade" tabindex="-1" role="dialog"></div>');
    var mdialog = $('<div class="modal-dialog modal-lg" role="document"></div>');
    var mcontent = $('<div class="modal-content"><div class="modal-header bg-inverse"><h5 class="modal-title"></h5></div></div>');
    var closeButton = $('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
    var mbody = $('<div class="modal-body"></div>');
    var mfooter = $('<div class="modal-footer"></div>');
    var resultFn = function () { };

    mcontent.find('.modal-header').append(closeButton);

    opt.closeButtonIndex = Math.min(opt.closeButtonIndex, opt.buttons.length - 1);

    if (opt.closeButtonIndex < 0) {
        closeButton.click(opt.buttons[opt.buttons.length - 1].click);
    } else {
        closeButton.click(opt.buttons[opt.closeButtonIndex].click);
    }

    if (opt.icon) {
        mbody.append($('<i class="fa-2x float-left mr-3" />').addClass(opt.icon));
    }
    mbody.append($('<div style="max-height: 280px; overflow: auto;" />').html(message));

    mcontent.find('.modal-title').text(opt.title);

    for (var b = 0; b < opt.buttons.length; b++) {
        var button = $('<button type="button" class="btn btn-secondary" data-dismiss="modal"></button>');

        if (opt.buttons[b].style) {
            button.removeClass('btn-secondary').addClass(opt.buttons[b].style);
        }

        button.html(opt.buttons[b].text);

        button.data('click', opt.buttons[b].click);

        button.click(function () {
            resultFn = $(this).data('click');
        });

        mfooter.append(button);
    }

    mcontent.append(mbody, mfooter);

    mdialog.append(mcontent);
    modal.append(mdialog);

    modal.on('hidden.bs.modal', function () {
        modal.remove();
        if (resultFn) resultFn();
    });

    $('body').append(modal);

    modal.modal({ backdrop: 'static' });
};

$(emmaliya).bind('emma.initialize', function (e, init_object) {

    /**********************************************************************
    PANEL

    Finds all objects with .emma-panel and dynamically generates
    a Bootstrap 4 .card object

    If a class of .emma-custom is also present, no automatic processing
    by the framework is done
    **********************************************************************/

    $(init_object).find('.emma-panel').each(function () {
        $(this).addClass('card');

        if (!this.id || this.id == '') {
            //this.id = '_rrc_panel_' + rrc.fn.counter();
        }

        if ($(this).find('> .card-body').length == 0) {
            $(this).wrapInner('<div class="card-body emma-source-target"></div>');

            if (!$(this).data('emma-source-target')) {
                $(this).data('emma-source-target', '#' + this.id + ' > .emma-source-target');
            }
        }

        if ($(this).find('> .card-header').length == 0) {
            var header = $('<div class="card-header"></div>');

            header.append($('<span />').text($(this).attr('title')));

            $(this).attr('title', null);

            $(this).prepend(header);
        }
    });


    /**********************************************************************
    POPUP

    Finds all objects with .emma-popup and dynamically generates
    a Bootstrap 4 .modal object
    **********************************************************************/
    $(init_object).find('.emma-popup').each(function () {        
        var popup = $(this);
        var content = '';

        if ($(this).find('.modal-content').length > 0) {
            content = $(this).find('.modal-content').html();
        } else {
            content = $(this).html();
            $(this).html('');
        }

        //If there are any codemirror objects on this popup, refresh them:
        popup.on('shown.bs.modal', function () {
            popup.find('.emma-codemirror').each(function () { $(this).data('CodeMirror').refresh(); });
            //Set focus to the first visible control in the modal's body:
            popup.find('.modal-body :input:visible').first().focus();
        });

        //We'll generate the popup unless it also has a class of .emma-custom
        if (!$(popup).is('.emma-custom')) {
            var modal_dialog = $('<div class="modal-dialog modal-lg" role="document"></div>');
            var modal_content = $('<div class="modal-content"></div>');
            var modal_header = $('<div class="modal-header bg-inverse"></div>');
            var modal_body = $('<div class="modal-body"></div>');
            var modal_footer = $('<div class="modal-footer"></div>');

            var modal_title = $('<h5 class="modal-title"></h5>');

            //modal_title.attr('id', '_rrc_modal_title_' + rrc.fn.counter());

            modal_title.html($(popup).attr("title"));

            modal_dialog.attr("aria-labelledby", modal_title.attr('id'));

            modal_header.append(modal_title, $('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'));

            modal_content.append(modal_header, modal_body, modal_footer);
            modal_dialog.append(modal_content);

            modal_body.html(content);

            var buttons = [{ Text: 'Save', Action: 'Save' }, { Text: 'Cancel', Action: 'Cancel' }];

            if ($(popup).attr('data-emma-buttons') && $(popup).attr("data-emma-buttons") != '') {
                buttons = eval('(' + $(popup).attr('data-emma-buttons') + ')');
            }

            for (var b = 0; b < buttons.length; b++) {
                var button = $('<button class="btn" />');

                switch (buttons[b].Action) {
                    case 'Save':
                        button.addClass('btn-success');
                        break;

                    case 'Cancel':
                        button.addClass('btn-warning');
                        break;
                }

                button.addClass(buttons[b].Button);

                button.attr('data-emma-action', buttons[b].Action);

                button.html(buttons[b].Text);

                modal_footer.append(' ', button);
            }

            if (modal_dialog.parent().length == 0) {
                popup.append(modal_dialog);
            }

            popup.addClass('modal fade').attr('tabindex', '-1').attr('role', 'dialog');
        }

        //Create a namespace for emma-specific functions:
        this.emmaliya = {};

        /*this.emma.newObject = function (className, initialValues, onAfterLoad) {
            popup.rrcNewDatabaseObject(className, initialValues || {}, function () {
                popup.find('h5.modal-title').html(popup.data('emma-add-title') || popup.attr('title'));
                popup.modal('show');
            }, rrc.fn.alert,
                onAfterLoad);
        };

        this.rrc.editObject = function (editId) {
            if (popup.data('emma-source') && popup.data('emma-source') != '') {
                var args = [editId];

                if ($(popup).attr('data-emma-source-args') && $(popup).attr('data-emma-source-args') != '') {
                    args = eval($(popup).attr('data-emma-source-args'));
                }

                //Invoke the method specified in data-emma-source:
                rrc.fn.invokeReferencedMethod($(popup).data('emma-source'), args,
                    function (result) {
                        var obj = jQuery.parseJSON(result);
                        popup.rrcDatabind(obj);
                        popup.find('h5.modal-title').html(popup.attr('title'));
                        //popup.rrcValidate();
                        popup.modal('show');
                    }, function (result) {
                        rrc.fn.alert(result);
                    }, function () {
                        $(popup).trigger('rrc.load-complete');
                    });
            } else {
                alert('Missing/invalid source on popup ' + popup.id);
            }

        };*/

        popup.bind('emma.action', function (e, a) {
            switch (a.action) {
                case 'Save':
                    //Disable all fields:
                    popup.find(':input,button').each(function () {
                        $(this).data('disabled-cache', $(this).prop('disabled'));
                        $(this).prop('disabled', true);
                    });

                    //Validate the form:
                    /*if (popup.rrcValidate()) {
                        var obj = popup.rrcUnbind();
                        var data = { handled: false };

                        popup.trigger('rrc.save', data);

                        if (!data.handled && popup.data('emma-update') && popup.data('emma-update') != '') {
                            var args = [];

                            if ($(popup).attr('data-emma-update-args') && $(popup).attr('data-emma-update-args') != '') {
                                args = eval($(popup).attr('data-emma-update-args'));
                            }

                            //Append the object to the arguments
                            args[args.length] = obj;

                            //Invoke the method specified in data-emma-source:
                            rrc.fn.invokeReferencedMethod($(popup).data('emma-update'), args,
                                function (result) {
                                    popup.trigger('emma.save-complete', result);
                                    popup.modal('hide');
                                    if (popup.data('emma-refresh') && popup.data('emma-refresh') != '') {
                                        $(popup.data('emma-refresh')).trigger('rrc.refresh');
                                    }

                                    $('*[data-emma-editor="' + popup.attr('id') + '"]').trigger('rrc.refresh');
                                }, function (result) {
                                    rrc.fn.alert(result);
                                    popup.trigger('emma.save-failed', result);
                                }, function () {
                                    popup.find(':input,button').prop('disabled', false);
                                });
                        }
                    } else {
                        //If we're not valid, reenable the fields that we're disabled beforehand:
                        //popup.find(':input,button').prop('disabled', false);
                        popup.find(':input,button').each(function () {
                            $(this).prop('disabled', $(this).data('disabled-cache'));
                            $(this).data('disabled-cache', null);
                        });
                    }*/
                    break;

                case 'Cancel':
                    popup.trigger('emma.cancelled');
                    popup.modal('hide');
                    break;
            }
        });

        //Handle the enterkey to trigger the first button in the footer,
        //except for textareas.
        //NOTE: We might have to do something for codemirror controls here, too!
        popup.emmaliya().enterKey(function () {
            if ($(':focus').is('textarea')) return true;

            modal_footer.find('button.btn-success, button.btn-warning').first().click();
        });

        popup.modal({ backdrop: 'static', show: false });
    });


});

$(document).ready(function () {
    
});