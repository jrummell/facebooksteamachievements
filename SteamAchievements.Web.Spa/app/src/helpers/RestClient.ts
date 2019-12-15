export default class RestClient {
    token?: string;

    headers: Headers = new Headers({
        "Content-Type": "application/json",
        Accept: "application/json"
    });

    setToken(token: string) {
        this.headers.set("Authorization", `Bearer ${token}`);
    }

    async getJson<TResponse>(url: string): Promise<TResponse | null> {
        const response = await fetch(url, {
            method: "GET",
            headers: this.headers
        });

        if (!response.ok) {
            return null;
        }

        return await response.json();
    }

    async postJson<TBody, TResponse = TBody>(url: string, body: TBody): Promise<TResponse> {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify(body),
            headers: this.headers
        });

        return await response.json();
    }

    async putJson<TBody, TResponse = TBody>(url: string, body: TBody): Promise<TResponse> {
        const response = await fetch(url, {
            method: "PUT",
            body: JSON.stringify(body),
            headers: this.headers
        });

        return await response.json();
    }

    async deleteJson<TBody, TResponse = TBody>(url: string, body: TBody): Promise<TResponse> {
        const response = await fetch(url, {
            method: "DELETE",
            body: JSON.stringify(body),
            headers: this.headers
        });

        return await response.json();
    }

    async postFormUrlEncoded<TResponse>(url: string, body: string): Promise<TResponse> {
        const response = await fetch(url, {
            method: "POST",
            body: body,
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        });

        return await response.json();
    }
}
