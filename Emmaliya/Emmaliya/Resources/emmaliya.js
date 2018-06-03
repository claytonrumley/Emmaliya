var emmaliya = emmaliya || {};

(function (emmaliya, $) {
    emmaliya.async = {};

    emmaliya.fn = {
        alert: function (msg) {
            alert(msg);
        },

        confirm: function (text, options, onOk, onCancel) {
            if (confirm(text)) {
                onOk();
            } else {
                onCancel();
            }
        },

        invokeAsync: function (invokePath, methodName, arguments, onSuccess, onFailure, onComplete) {
            jQuery.ajax({
                method: 'POST',
                data: { __asyncmethod: methodName, arguments: JSON.stringify(arguments) },
                dataType: 'json',
                success: function (data) {
                    if (data.Success) {
                        if (onSuccess) onSuccess(data.Result);
                    } else {
                        if (onFailure) onFailure(data.Exception);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (onFailure) onFailure(errorThrown);
                },
                url: invokePath
            }).done(function () {
                if (onComplete) onComplete();
            });
            //__asyncmethod
        }
    };

    emmaliya.currentApplication = {};
    emmaliya.currentUser = {};
    emmaliya.currentPage = {};

    emmaliya.initialize = function (init_object) {

        /*******************************
         * data-emma-action
         * ****************************/
        init_object.find('*[data-emma-action]').click(function () {
            var target = $(this);

            if ($(this).attr('data-emma-target') && $(this).attr('data-emma-target') !== '') target = $($(this).attr('data-emma-target'));

            target.trigger('emma.action', { element: this, target: target, action: $(this).attr('data-emma-action'), id: $(this).attr('data-emma-id') });
        });

        /*******************************
         * data-emma-source
         * ****************************/
        init_object.find('*[data-emma-source]:not(.modal)').each(function () {
            var target = $(this);

            $(this).bind('emma.action', function (e, a) {
                switch (a.action) {
                    case 'Refresh':
                        e.stopPropagation();

                        a.target.trigger('emma.refresh');

                        break;
                }
            }).bind('emma.refresh', function (e) {
                var pieces = target.attr('data-emma-source').split('.');
                var fn = emmaliya.async;
                var prev;
                var args = [];

                if (target.attr('data-emma-source-args') && target.attr('data-emma-source-args') != '') {
                    args = eval(target.attr('data-emma-source-args'));
                }

                //OnSuccess
                args[args.length] = function (result) {
                    target.html(result);
                };

                //OnFailure
                args[args.length] = emmaliya.fn.alert;

                for (var p = 0; p < pieces.length; p++) {
                    prev = fn;
                    fn = fn[pieces[p]];
                }

                fn.apply(prev, args);
            });

        });

        init_object.find('*[data-emma-source]').not('.emma-nil').trigger('emma.refresh');
    };

    $(document).ready(function () {
        emmaliya.initialize($('body'));
    });

})(emmaliya, jQuery);