$(document).ready(function () {

    // REGISTER USER

    function toggleCourse() {

        if ($("#Role").val() == "2") {
            $("#courseDiv").show();
        }
        else {
            $("#courseDiv").hide();
            $("#Course").val("");
        }
    }

    $("#Role").change(function () {
        toggleCourse();
    });

    toggleCourse();

    $("#registerForm").submit(function (e) {

        e.preventDefault();

        $.ajax({
            url: '/Admin/Admin/RegisterUser',
            type: 'POST',
            data: $(this).serialize(),

            success: function (response) {
                alert(response.message);
            },

            error: function (xhr) {
                alert("Error: " + xhr.status);
                console.log(xhr.responseText);
            }
        });

    });

});