$(document).ready(function() {
    $(function() {
        $(".carousel-img-container").height($(window).height());
        $("#carousel-example-generic").carousel({
            pause: "false"
        });
    });

    $(window).resize(function() {
        $(".carousel-img-container").height($(window).height());
    });
    
    $("body").on("click",".video-anchor", function (){
        $("#iframe-watch").html('<iframe class="embed-responsive-item" width="640" height="480" src="https://www.youtube.com/embed/'+$(this).attr("data-video")+'" frameborder="0" allowfullscreen></iframe>');
        $("#video-title").html($(this).attr("data-title"));
    });
    
    $('#videoModal').on('hidden.bs.modal', function() {
        $("#iframe-watch").html('');
    });
});