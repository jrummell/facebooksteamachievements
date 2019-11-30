module.exports = {
    root: true,
    env: {
        node: true
    },
    extends: ["plugin:vue/essential", "@vue/prettier", "@vue/typescript"],
    rules: {
        "no-console": "warn",
        "no-debugger": "warn",
        "quotes": [
            "warn",
            "double"
        ],
        "indent": [
            "warn",
            4
        ],
        "no-tabs": "error"
    },
    parserOptions: {
        parser: "@typescript-eslint/parser"
    },
    overrides: [
        {
            files: [
                "**/__tests__/*.{j,t}s?(x)",
                "**/tests/unit/**/*.spec.{j,t}s?(x)"
            ],
            env: {
                jest: true
            }
        }
    ]
};
