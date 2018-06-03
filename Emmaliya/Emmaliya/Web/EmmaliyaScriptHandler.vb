Namespace Web
    Friend Class EmmaliyaScriptHandler
        Inherits RouteHandler

        Public Overrides Sub ProcessRequest()
            Response.ContentType = "application/javascript"

            Response.Write(My.Resources.emmaliya)

            '            Dim script As String = <script>
            'var emmaliya = emmaliya || {};

            '(function(emmaliya, $) {
            '    emmaliya.async = {};

            '    emmaliya.fn = {
            '        alert: function(msg) { 
            '            alert(msg);
            '        },

            '        confirm: function(text, options, onOk, onCancel) { 
            '            if(confirm(text)) { 
            '                onOk();
            '            } else { 
            '                onCancel();
            '            }
            '        },

            '        invokeAsync: function (invokePath, methodName, arguments, onSuccess, onFailure, onComplete) {
            '            jQuery.ajax({  
            '                method: 'POST',
            '                data: { __asyncmethod: methodName, arguments: JSON.stringify(arguments) },
            '                dataType: 'json',
            '                success: function (data) {
            '                    if (data.Success) {
            '                        if (onSuccess) onSuccess(data.Result);
            '                    } else {
            '                        if (onFailure) onFailure(data.Exception);
            '                    }
            '                },
            '                error: function (jqXHR, textStatus, errorThrown) {
            '                    if (onFailure) onFailure(errorThrown);
            '                },
            '                url: invokePath
            '            }).done(function () {
            '                if (onComplete) onComplete();
            '            });
            '            //__asyncmethod
            '        }
            '    };

            '    emmaliya.currentApplication = {};
            '    emmaliya.currentUser = {};
            '    emmaliya.currentPage = {};

            '    emmaliya.initialize = function (el) {
            '        //[[initialize]]

            '        el.find('*[data-emmaliya-source]').not('.emmaliya-nil').trigger('emmaliya.refresh');
            '    };

            '    $(document).ready(function () {
            '        emmaliya.initialize($('body'));
            '    });

            '})(emmaliya, jQuery);
            '</script>.Value



        End Sub
    End Class
End Namespace