// Код до рефакторингу
class UserManager {
    constructor() {
        this.users = [];
    }

    addUser(user) {
        if (this.validateUser(user)) {
            this.users.push(user);
            console.log("User added successfully.");
        } else {
            console.log("Invalid user.");
        }
    }

    validateUser(user) {
        return user && user.name && user.email;
    }

    listUsers() {
        console.log("User List:");
        this.users.forEach(user => console.log(`${user.name} (${user.email})`));
    }
}

const manager = new UserManager();
manager.addUser({ name: "Anna", email: "anna@example.com" });
manager.listUsers();
// validateUser доступний зовні, хоча це непотрібно
console.log(manager.validateUser({ name: "", email: "test@example.com" })); // Неправильний виклик

// Код після рефакторингу (Hide Method)
class UserManager {
    constructor() {
        this.users = [];
    }

    addUser(user) {
        if (this.#validateUser(user)) {
            this.users.push(user);
            console.log("User added successfully.");
        } else {
            console.log("Invalid user.");
        }
    }

    // Робимо метод приватним
    #validateUser(user) {
        return user && user.name && user.email;
    }

    listUsers() {
        console.log("User List:");
        this.users.forEach(user => console.log(`${user.name} (${user.email})`));
    }
}

// Код до рефакторингу
function calculateAreaProperties(radius, height) {
    let temp = Math.PI * radius * radius;
    console.log(`Base Area: ${temp}`);

    temp = 2 * Math.PI * radius * height;
    console.log(`Lateral Surface Area: ${temp}`);

    temp = temp + 2 * Math.PI * radius * radius;
    console.log(`Total Surface Area: ${temp}`);

    temp = Math.PI * radius * radius * height;
    console.log(`Volume: ${temp}`);
}

// Код після рефакторингу (Split Temporary Variable)
function calculateAreaProperties(radius, height) {
    const baseArea = Math.PI * radius * radius;
    console.log(`Base Area: ${baseArea}`);

    const lateralSurfaceArea = 2 * Math.PI * radius * height;
    console.log(`Lateral Surface Area: ${lateralSurfaceArea}`);

    const totalSurfaceArea = lateralSurfaceArea + 2 * baseArea;
    console.log(`Total Surface Area: ${totalSurfaceArea}`);

    const volume = baseArea * height;
    console.log(`Volume: ${volume}`);
}

// Код до рефакторингу
function isWeekend(date) {
    let dayOfWeek = date.getDay();
    let isWeekend = dayOfWeek === 0 || dayOfWeek === 6;
    return isWeekend;
}

// Код після рефакторингу (Replace Temp with Query)
function isWeekend(date) {
    return date.getDay() === 0 || date.getDay() === 6;
}
