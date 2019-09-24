module.exports = {
    dist: {
        options: {
            sourceMap: true,
        },
        files: [
            {
                expand: true,
                cwd: '<%= paths.source.stylesheets %>',
                src: ['*.less'],
                dest: '<%= paths.public.stylesheets %>',
                ext: '.css',
            },
        ],
    },
};
