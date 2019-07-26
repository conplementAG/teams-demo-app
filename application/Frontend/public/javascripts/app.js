'use strict';

const e = React.createElement;

class AppRoot extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            items: [],
            version: window.VERSION,
            backend_version: "unknown",
            backend_host: window.BACKEND_HOST
        };

        this.addTeamMember = this.addTeamMember.bind(this);
    }

    componentDidMount() {
        fetch(this.state.backend_host + "/api/team")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        teamNames: result
                    });
                },
                (error) => {
                    console.error("Error loading teams")
                    console.error(error)
                    this.setState({
                        isLoaded: true,
                        error: "Could not load teams"
                    });
                }
            )

        fetch(this.state.backend_host + "/api/version")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({ backend_version: result.major + "." + result.minor + "." + result.build + "." + result.revision });
                },
                (error) => {
                    console.error("Error loading backend version")
                    console.error(error)
                }
            )

    }

    selectTeam(teamName) {
        if (this.state.teamNames !== undefined) {
            fetch(this.state.backend_host + "/api/team/" + teamName)
                .then(res => res.json())
                .then(
                    (result) => {
                        this.setState({
                            selectedTeam: result
                        });
                    },
                    (error) => {
                        console.error("Error loading team members")
                        console.error(error)
                        this.setState({ error: "Could not load team members" });
                    }
                )
        }
    }

    isTeamSelected(teamName) {
        if (this.state.selectedTeam === undefined) {
            return false;
        }

        return this.state.selectedTeam.teamName === teamName;
    }

    deleteMember(name, role, teamName) {
        fetch(this.state.backend_host + "/api/team/" + teamName,
            {
                method: 'delete',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: name, role: role })
            })
            .then(() => {
                this.selectTeam(teamName)
            },
                (error) => {
                    console.error("Error deleting team member")
                    console.error(error)
                    this.setState({ error: "Could not delete team member" });
                }
            )
    }

    addTeamMember(event) {
        console.log("adding team member");
        event.preventDefault();
        const data = new FormData(event.target);

        let teamName = this.state.selectedTeam.teamName;
        fetch(this.state.backend_host + '/api/team/' + teamName, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: data.get('name'), role: data.get('role') })
        }).then(() => {
            this.selectTeam(teamName)
        },
            (error) => {
                console.error("Error adding a new team member")
                console.error(error)
                this.setState({ error: "Could not add a new team member" });
            }
        )
    }

    render() {

        let that = this;
        let teamSelection;
        if (!this.state.isLoaded) {
            teamSelection = (<span className="badge badge-info">Loading ...</span>)
        } else if (this.state.isLoaded && this.state.error != undefined) {
            teamSelection = (
                <div>
                    <span className="badge badge-danger">Error:</span>
                    <div>{ this.state.error }</div>
                </div>
            )
        } else {
            teamSelection = (
                <div>
                    <small>Please select a team ...</small>

                    {
                        this.state.teamNames.map(function (teamName, i) {
                            let selectedClass = "";
                            if (that.isTeamSelected(teamName)) {
                                selectedClass = " selected"
                            }
                            return (<div key={ i } className={ "box" + selectedClass } onClick={ () => that.selectTeam(teamName) }>
                                <a href="#">{ teamName }</a> <i className='fas fa-angle-right'></i>
                            </div>)
                        })
                    }
                </div>
            )
        }

        let teamExtension;

        if (this.state.selectedTeam !== undefined) {
            teamExtension = (
                <div className="group">
                    <h4>Add members to { this.state.selectedTeam.teamName }</h4>

                    <form onSubmit={ this.addTeamMember }>
                        <div className="form-group row">
                            <label htmlFor="name" className="col-sm-2 col-form-label">Name</label>
                            <div className="col-sm-10">
                                <input className="form-control" id="name" name="name" type="text" required />
                            </div>
                        </div>
                        <div className="form-group row">
                            <label htmlFor="name" className="col-sm-2 col-form-label">Role</label>
                            <div className="col-sm-10">
                                <input className="form-control" id="name" name="role" type="text" required />
                            </div>
                        </div>
                        <div className="form-group row">
                            <div className="col-sm-12">
                                <button type="submit" className="btn btn-primary">Add new member</button>
                            </div>
                        </div>
                    </form>
                </div>)
        } else {
            teamExtension = <div></div>;
        }

        let teamMembers

        if (this.state.selectedTeam !== undefined) {
            teamMembers = (
                <div>
                    <h3>Team Members</h3>
                    { teamExtension }
                    { this.state.selectedTeam.members.map(function (member, i) {
                        return (
                            <div className="box" key={ i }>
                                <div className="row">
                                    <div className="col-sm-10">
                                        Name: { member.name }<br />
                                        Role: { member.role }<br />
                                    </div>
                                    <div className="col-sm-2">
                                        <div className="float-right">
                                            <button onClick={ () => that.deleteMember(member.name, member.role, that.state.selectedTeam.teamName) } type="button" className="btn btn-danger"><i className="fas fa-trash"></i></button>
                                        </div>
                                    </div>
                                </div>
                            </div>)
                    }) }
                </div>
            )
        } else if (this.state.selectedTeam === undefined && this.state.error === undefined) {
            teamMembers = (<div>Please select a team</div>)
        } else {
            teamMembers = (<div></div>)
        }

        return (
            <div>
                <h1>Azure Kubernetes Workshop</h1>
                <h2>Demo Application <span className="badge badge-secondary">{ this.state.version }</span></h2>

                <hr />

                <div id="configurable">
                    <p><i className="fas fa-info-circle"></i> The color can be configured via environment variable: <strong>COLOR</strong></p>
                </div>
                <div className="content">
                    <small><i className="fas fa-info-circle"></i> Here some ajax request will be performed</small><br />
                    <a href={ this.state.backend_host + "/health/ready" } target="_blank">Backend: { this.state.backend_host }
                        <span className="badge badge-secondary">{ this.state.backend_version }</span>
                    </a>
                    <a href={ this.state.backend_host + "/swagger" } target="_blank">
                        Swagger
                        <img src="images/swagger.jpg" alt="Swagger" title="Swagger" />
                    </a>
                    <div>
                        <h3>Teams</h3>
                        { teamSelection }
                    </div>
                    <div>
                        { teamMembers }
                    </div>
                </div>
            </div>
        );
    }
}

const domContainer = document.querySelector('#app');
ReactDOM.render(e(AppRoot), domContainer);