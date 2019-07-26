using Backend.Model;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Repository
{
    public class CompanyRepository
    {
        private List<Team> _teams = new List<Team>();

        public CompanyRepository()
        {
            var teamRepo = new TeamRepository();

            _teams.Add(teamRepo.CreateDeveloperTeam());
            _teams.Add(teamRepo.CreateDevOpsTeam());
            _teams.Add(teamRepo.CreateOperationsTeam());
        }

        public Team GetTeamByName(string name)
        {
            return _teams
                .Where(t => t.TeamName == name)
                .FirstOrDefault();
        }

        public List<string> GetAvailableTeams()
        {
            return _teams
                .Select(t => t.TeamName)
                .ToList();
        }

        public void AddTeam(string teamName)
        {
            lock (_teams)
            {
                _teams.Add(new Team() {
                    TeamName = teamName,
                    Members = new List<TeamMember>()}
                );
            }
        }

        public void DeleteTeam(string teamName)
        {
            lock(_teams)
            {
                _teams.RemoveAll(t => string.Compare(t.TeamName, teamName, true) == 0);
            }
        }

        public void AddTeamMember(TeamMember teamMember, string teamName)
        {
            var team = _teams.FirstOrDefault(t => t.TeamName == teamName);
            if(team != null)
            {
                lock (team)
                {
                    team.Members.Add(teamMember);
                }
            }
        }

        public void RemoveTeamMemer(TeamMember teamMember, string teamName)
        {
            var team = _teams.FirstOrDefault(t => t.TeamName == teamName);
            if (team != null)
            {
                lock (team)
                {
                    team.Members.Remove(teamMember);
                }
            }
        }
    }
}
