import React, { Component } from "react";
import PropTypes from 'prop-types';
import _ from 'underscore';

export default class UserDetailsMatches extends Component {
    static get propTypes() {
        return {
            games: PropTypes.array.isRequired,
        };
    }

    static get defaultProps() {
        return {
            games: [],
        };
    }

    renderMatchesRows() {
        let html = [];

        _.map(this.props.games, (match, index) => {
            html.push(
                <tr className="tr-color-userDetails" key={`match_${index}`}>
                    <td >{index}</td>
                    <td>{match.date}</td>
                    <td>{match.level}</td>
                    <td>{match.correctanswers}</td>
                    <td>{match.totalhelp || 0}</td>
                    <td>{match.totaltime}</td>
                </tr>
            );
        });

        return html;
    }

    render() {
        return (
            <div className="container-tableData-userDetails">
                <div className="rowTable-userDetails">
                    <div className="col-md-12 col-lg-12">
                        <div className="panel panel-primary">
                            <div className="panel-heading-table">
                                <span className="panel-title-table">Partite</span>
                                <span className="table-count">Totale : {this.props.games.length}</span>
                            </div>

                            <table className="table sortable">
                                <thead>
                                    <tr className="tr-userDetails">
                                        <th className="nosort" >#</th>
                                        <th className="nosort" >Data</th>
                                        <th className="nosort" >Livello</th>
                                        <th className="nosort" >Risposte Esatte</th>
                                        <th className="nosort" >Aiuti</th>
                                        <th className="nosort" >Tempo</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this.renderMatchesRows()}
                                </tbody>
                            </table>
                        </div>
                    </div>

                </div>
            </div>
        );
    }
}