export default interface FB {
    api(url: string, method: string, body: {}, callback: (user: { id: string }) => void): void;
    ui(options: object, callback: (response: object) => Promise<void>): void;
}
