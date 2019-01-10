$(document).ready(function () {
    var scroll = $('#msg-scrollable-window');
    scroll.scrollTop = scroll.scrollHeight;
    setInterval(load_messes, 3000);

    $('#button').click(function () {
        var targetId = $('#targetId').val();
        var msg = $('#msg').val();
        var cur = $('#curSelect').val();

        if (msg !== "")
            $.ajax({
                type: 'POST',
                url: '/Messages/Write',
                data: { targetId: targetId, msg: msg, cur: cur },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (res) {
                    if (res === false)
                        alert("нет средств!");
                    else {
                        var li = '<li>' + res + '</li>';
                        $('#uml').append(li);
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
                $("#uml").append(msgs);

                // опустить сообщения вниз
                var scroll = $('#msg-scrollable-window');
                scroll.scrollTop = scroll.scrollHeight;
            }
        }
    });
}

