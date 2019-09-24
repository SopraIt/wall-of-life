import React, { Component } from "react";
import _ from 'underscore';
import moment from 'moment';
import classNames from 'classnames';
// import { Link } from "react-router-dom";
import { Redirect } from 'react-router';
import Header from './Header';
import InformationsStore from '../stores/InformationsStore';
import InformationsActions from '../actions/InformationsActions';

export default class Search extends Component {
    constructor(props) {
        super(props);
        this.state = { users: InformationsStore.people(), search: '', deleteModal: false, userToDelete: '' };
        this.onInformationsLoaded = this.onInformationsLoaded.bind(this);
        this.changeCriteria = this.changeCriteria.bind(this);
        this.deleteUser = this.deleteUser.bind(this);
    }

    componentWillMount() {
        InformationsStore.addInformationsListener(this.onInformationsLoaded);
        InformationsActions.getInformations();
    }

    componentWillUnmount() {
        InformationsStore.removeInformationsListener(this.onInformationsLoaded);
    }

    onInformationsLoaded() {
        this.setState({ users: InformationsStore.people() });
    }

    goToDetails(userId) {
        this.setState({ redirect: true, userToRedirect: userId });
    }
    
    openCloseDeleteModal(e, status, userId) {
        e.stopPropagation();

        const userToDelete = status && userId ? userId : '';

        this.setState({ deleteModal: status, userToDelete });
    }

    deleteUser() {
        InformationsActions.deleteUser(this.state.userToDelete);
        this.setState({ deleteModal: false, userToDelete: '' });
    }

    renderUsers() {
        const html = [];
        _.map(this.state.users, (user, index) => {
            html.push(
                <tr className="tr-color-userDetails" key={user.id} onClick={() => this.goToDetails(user.id)}>
                    <td> {index} </td>
                    <td> {user.firstName} </td>
                    <td> {user.lastName} </td>
                    <td> {moment(user.dateOfBirth).format('DD/MM/YYYY')} </td>
                    <td> {user.level || 1} </td>
                    <td className="delete" onClick={(e) => this.openCloseDeleteModal(e, true, user.id)}> X </td>
                </tr>
            );
        });
        return html;
    }

    changeCriteria(event) {
        const criteria = event.target.value;
        this.setState({ search: criteria });
        InformationsActions.searchAction(criteria);
    }

    render() {
        const deleteOverlayClass = classNames({
            'modal-delete-user modal fade in': true,
            open: this.state.deleteModal,
        });

        if (this.state.redirect) {
            return <Redirect push to={`/${this.state.userToRedirect}`} />;
        }
        return (
            <div>
                <Header/>
                <div className="search_page container">
                    <div className="search_section">
                        <input className="search_input " type="text" id="search" placeholder="Cerca..." onChange={this.changeCriteria} value={this.state.search} />
                        <img className="img-button-create" src="images/loupe.png" onClick={() => InformationsActions.searchAction(this.state.search)} />
                    </div>
                    <div className="panel panel-primary users_table">
                        <div className="panel-heading-table table_header">
                            <span className="panel-title-table"> Utenti </span>
                            <span className="table-count"> Totali: {this.state.users.length} </span>
                        </div>
                        <table className="table sortable">
                            <thead>
                                <tr className="tr-userDetails">
                                    <th> ID </th>
                                    <th> Nome </th>
                                    <th> Cognome </th>
                                    <th> Data di Nascita </th>
                                    <th> Livello </th>
                                    <th className="delete"> Rimuovi </th>
                                </tr>
                            </thead>
                            <tbody >
                                {this.renderUsers()}
                            </tbody >
                        </table>
                    </div>
                </div>
                <div className={deleteOverlayClass} tabIndex="-1" role="dialog">
                  <div className="modal-dialog modal-dialog-centered" role="document">
                      <div className="modal-content">
                          <div className="modalHeaderuser">
                              <h1 className="modal-title" id="overlay">Vuoi davvero cancellare questo utente?</h1>
                          </div>
                          <div className="modal-body">
                            <div className="button-container">
                              <button onClick={(e) => this.openCloseDeleteModal(e, false)}>
                                  <img className="abort-delete" src="images/cancel.png" />
                              </button>
                              <button onClick={this.deleteUser}>
                                  <img className="confirm-delete" src="images/checked.png" />
                              </button>
                            </div>
                          </div>
                      </div>
                  </div>
                </div>
            </div>
        );
    }
}
