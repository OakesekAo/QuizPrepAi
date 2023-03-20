$(document).ready(function() {
    var currentQuestion = 0;
    var totalQuestions = $(".quiz-question").length;

    // Hide all questions except the first one
    $(".quiz-question:not(:first)").hide();

    // Handle form submission
    $("#quiz-form").submit(function(event) {
        event.preventDefault();

        // Get the selected answer
        var selectedAnswer = $("input[name='answer-" + currentQuestion + "']:checked").val();

        // Show the next question, or redirect to the results page if all questions have been answered
        if (currentQuestion < totalQuestions - 1) {
            // Hide the current question
            $("#question-" + currentQuestion).hide();

            // Show the next question
            currentQuestion++;
            $("#question-" + currentQuestion).show();
        } else {
            // Redirect to the results page
            window.location.href = "/Quiz/Results";
        }
    });
});


// assume you have a button with id="show-explanation"
$('#show-explanation').click(function () {
    // get the question ID from the button data attribute
    var questionId = $(this).data('question-id');

    // make the AJAX call to retrieve the explanation from the server
    $.ajax({
        url: '/Quiz/GetExplanation',
        method: 'POST',
        data: { questionId: questionId },
        success: function (response) {
            // display the explanation in a modal or other element
            $('#explanation-modal .modal-body').html(response);
            $('#explanation-modal').modal('show');
        },
        error: function (xhr, status, error) {
            // handle the error
            console.error(xhr.responseText);
        }
    });
});

