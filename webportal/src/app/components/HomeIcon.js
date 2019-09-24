import React, { Component } from "react";
import PropTypes from 'prop-types';
import { Link } from "react-router-dom";

export default class HomeIcon extends Component {
    static get propTypes() {
        return {
            icon: PropTypes.object.isRequired,
        };
    }

    static get defaultProps() {
        return {
            icon: {},
        };
    }

    render() {
        return (
            <div className="col-xs-12 col-sm-4 home-icon">
                <Link to={this.props.icon.url}>
                    <button type="button" className="btn btn-light button-user">
                        <img className="text-awards" src={`images/${this.props.icon.img}`} />
                        <h1 className="text-awards" dangerouslySetInnerHTML={{ __html: this.props.icon.label }} />
                    </button>
                </Link>
            </div>
        );
    }
}
