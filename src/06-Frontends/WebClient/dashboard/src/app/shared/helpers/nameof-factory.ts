export const nameofFactory = <T>(lowerCaseKey: boolean = false) =>
    (name: keyof T, _lowerCaseKey: boolean = lowerCaseKey) =>
        _lowerCaseKey ? name.toString().toLocaleLowerCase() : name;
