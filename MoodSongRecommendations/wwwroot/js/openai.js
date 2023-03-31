$('#btnOpenAIPrompt').click(function (e) {
    let result = $("#presult");

    var mod = $('#mod').val();
    if (mod != null || mod != "") {
        debugger;
        $.ajax({
            url: '/Home/GetSongRecommendations',
            method: 'POST',
            content: "application/json; charset=utf-8",
            data: { mod: mod },
            success: function (d) {
                const songs = d.split('\n');

                for (let i = 0; i < songs.length; i++) {
                    if (songs[i] != "") {
                        result.append(songs[i]+"</br>");
                        loadYoutubeSong(songs[i]);
                      
                    }
                }
            }
        });
    }


});

function loadYoutubeSong(songName) {
    $.ajax({
        url: '/Home/GetYoutubeSong',
        method: 'GET',
        content: "application/json; charset=utf-8",
        dataType: 'json',
        data: { songName: songName },
        success: function (d) {
           
            var x = document.createElement("IFRAME");
            x.setAttribute("src", "https://www.youtube.com/embed/" + d.value[0].id.videoId);
            x.setAttribute("style", "margin: 10px 10px 10px 10px");
            document.body.appendChild(x);
            
        }
    });
}