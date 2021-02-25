module.exports = {
    root: true,
    env: {
        node: true
    },
    extends: [
        "plugin:vue/vue3-essential",
        "eslint:recommended",
        "@vue/typescript/recommended",
        "@vue/prettier",
        "@vue/prettier/@typescript-eslint"
    ],
    parserOptions: {
        ecmaVersion: 2020
    },
    rules: {
        "no-console": "warn",
        "no-debugger": "warn",
        quotes: ["warn", "double"],
        indent: ["warn", 4],
        "no-tabs": "error",
        "max-len": ["warn", { code: 120, ignoreStrings: true, ignoreTemplateLiterals: true, ignoreUrls: true }],
        "@typescript-eslint/interface-name-prefix": "off"
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
