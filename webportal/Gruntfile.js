const timeGrunt = require('time-grunt');
const loadGruntConfig = require('load-grunt-config');

module.exports = (grunt) => {
    const data = {
        paths: {
            source: {
                root: 'src',
                stylesheets: '<%= paths.source.root %>/stylesheets',
                javascripts: '<%= paths.source.root %>',
            },
            public: {
                root: 'electron_application_builder/public',
                stylesheets: '<%= paths.public.root %>/stylesheets',
                javascripts: '<%= paths.public.root %>/javascripts',
                builds: '<%= paths.public.root %>/builds',
            },
        },
    };
    // require it at the top and pass in the grunt instance
    timeGrunt(grunt);
    loadGruntConfig(grunt, {
        data,
    });
};
