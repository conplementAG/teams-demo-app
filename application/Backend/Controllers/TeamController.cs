using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Model;
using Backend.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private static readonly CompanyRepository _companyRepo = new CompanyRepository();

        /// <summary>
        /// Gets all available teams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _companyRepo.GetAvailableTeams();
        }

        /// <summary>
        /// Gets all members of a specific team
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        [HttpGet("{teamName}")]
        public ActionResult<Team> GetTeam(string teamName)
        {
            var team = _companyRepo.GetTeamByName(teamName);

            if (team == null)
            {
                return NotFound();
            }
            return team;
        }

        /// <summary>
        /// Adds another team
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        [HttpPost("{teamName}")]
        public ActionResult AddTeam(string teamName)
        {
            _companyRepo.AddTeam(teamName);
            return Ok();
        }

        /// <summary>
        /// Adds a member to a specific team
        /// </summary>
        /// <param name="teamMember"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        [HttpPut("{teamName}")]
        public ActionResult AddTeamMember(TeamMember teamMember, string teamName)
        {
            _companyRepo.AddTeamMember(teamMember, teamName);
            return Ok();
        }

        /// <summary>
        /// Deletes a member from a specific team
        /// </summary>
        /// <param name="teamMember"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        [HttpDelete("{teamName}")]
        public ActionResult RemoveTeamMember(TeamMember teamMember, string teamName)
        {
            _companyRepo.RemoveTeamMemer(teamMember, teamName);
            return Ok();
        }
    }
}
