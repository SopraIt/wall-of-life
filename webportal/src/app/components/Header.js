import React, { Component } from "react";
import PropTypes from 'prop-types';
import { Link } from "react-router-dom";

export default class Header extends Component {
    static get propTypes() {
        return {
            home: PropTypes.bool.isRequired,
            title: PropTypes.string,
            backUrl: PropTypes.string,
        };
    }

    static get defaultProps() {
        return {
            home: false,
            title: 'Wall of Life',
            backUrl: '/',
        };
    }

    renderBack() {
        let html = null;

        if (!this.props.home) {
            html = <Link className="index" to={this.props.backUrl}> <span className="back-arrow-container"><button className="back-arrow" /></span> </Link>;
        }

        return html;
    }

    render() {
        return (
            <header className="header">
                <div className="navigation">
                    { this.renderBack() }
                    <a className="prize">{this.props.title}</a>
                </div>
            </header>
        );
    }
}
