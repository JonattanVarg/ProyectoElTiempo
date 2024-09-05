namespace API.Configurations.InitialUsers
{
    public class InitialUsersConfig
    {
        public InitialUser InitialAdministrador { get; set; } = new InitialUser();
        public InitialUser InitialReclutador { get; set; } = new InitialUser();
        public InitialUser InitialCandidato { get; set; } = new InitialUser();
    }
}
