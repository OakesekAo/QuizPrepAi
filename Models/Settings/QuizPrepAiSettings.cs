namespace QuizPrepAi.Models.Settings
{
    public class QuizPrepAiSettings
    {
        public string OpenAiAPIKey { get; set; }
    }

    //Add other general get set here


    public class DefaultCredentials
    {
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
