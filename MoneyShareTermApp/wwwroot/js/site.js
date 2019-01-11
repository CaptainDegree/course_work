$(document).ready(function () {
    $('#msg-scrollable-window').scrollTop($('#uml').height());
    setInterval(load_messes, 1000);
    //$('#msg').keypress(function (e) {
    //    if (e.which == 13) {
    //        jQuery(this).blur();
    //        jQuery('#button').focus().click();
    //    }
    //});

    $('#button').click(function () {
        var targetId = $('#targetId').val();
        var msg = $('#msg').val();
        var cur = $('#curSelect').val();
        var type = $('#typeSelect').val();

        if (msg !== "")
            $.ajax({
                type: 'POST',
                url: '/Messages/Write',
                data: { targetId: targetId, msg: msg, cur: cur, type: type },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (res) {
                    if (res === false)
                        alert("ошибка!");
                    else {
                        var li = '<li>' + res + '</li>';
                        $('#uml').append(li);
                        $('#msg-scrollable-window').animate({ scrollTop: $('#uml').height() }, 1000);
                    }
                }
            })
    });
})

function load_messes() {
    var targetId = $('#targetId').val();
    var lastMsgId = $('input.msgId').last().val();

    $.ajax({
        type: "POST",
        url: "/Messages/SendMsgs",
        data: { targetId: targetId, msgId: lastMsgId },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },

        success: function (msg) {
            if (msg !== false) {
                // внести сообщения
                $("#uml").append('<li>' + msg + '</li>');

                // опустить сообщения вниз
                $('#msg-scrollable-window').animate({ scrollTop: $('#uml').height() + 300 }, 1000);
            }
        }
    });
}

