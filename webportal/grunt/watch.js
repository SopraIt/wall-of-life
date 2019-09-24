module.exports = {
    javascripts: {
        files: [
            '<%= paths.source.javascripts %>/**/*.js',
        ],
        tasks: ['javascripts_dev'],
        options: {
            spawn: false,
        },
    },
};
