module.exports = {
    root: true,
    env: {
        node: true
    },
    plugins: ["prettier"],
    extends: ["plugin:vue/essential", "prettier", "@vue/prettier", "@vue/typescript"],
    rules: {
        "no-console": "warn",
        "no-debugger": "warn",
        quotes: ["warn", "double"],
        indent: ["warn", 4],
        "no-tabs": "error",
        "max-len": ["warn", { code: 120, ignoreStrings: true, ignoreTemplateLiterals: true, ignoreUrls: true }]
    },
    parserOptions: {
        parser: "@typescript-eslint/parser"
    },
    overrides: [
        {
            files: ["**/__tests__/*.{j,t}s?(x)", "**/tests/unit/**/*.spec.{j,t}s?(x)"],
            env: {
                jest: true
            }
        }
    ]
};
