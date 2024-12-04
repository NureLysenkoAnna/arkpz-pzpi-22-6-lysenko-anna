// Оголошення та іменування констант (гарний приклад)
const RECTANGLE_NAME = "Rectangle";
//const PI = 3.14;

// Іменування константи (поганий приклад)
//const pi = 3.14;

/**
 * Calculates the area of a rectangle.
 * @param {number} width - The width of the rectangle.
 * @param {number} height - The height of the rectangle.
 * @returns {number} - The area of the rectangle.
 */

// Оголошення та іменування функції (гарний приклад)
// Функція обчислення площі прямокутника.
function calculateArea(width, height) {
  return width * height;
}

/* Іменування функції (поганий приклад)
function cA(w, h) {
  return w * h;
}*/
  
/**
 * Calculates the perimeter of a rectangle.
 * @param {number} width - The width of the rectangle.
 * @param {number} height - The height of the rectangle.
 * @returns {number} - The perimeter of the rectangle.
 */

function calculatePerimeter(width, height) {
  return 2 * (width + height);
}
  
/**
 * Validates the dimensions of a rectangle.
 * @param {number} width - The width of the rectangle.
 * @param {number} height - The height of the rectangle.
 * @returns {boolean} - True if dimensions are valid, false otherwise.
 */
  
function validateDimensions(width, height) {
  return width > 0 && height > 0;
}
  
/**
 * Represents a rectangle with width and height.
 */
class Rectangle {
  constructor(width, height) {
    this.width = width;
    this.height = height;
  }
  
  /**
   * Calculates the area of the rectangle using instance variables.
   * @returns {number} - The area of the rectangle.
   */
  calculateArea() {
    return calculateArea(this.width, this.height);
  }
  
  /**
   * Calculates the perimeter of the rectangle using instance variables.
   * @returns {number} - The perimeter of the rectangle.
   */
  calculatePerimeter() {
    return calculatePerimeter(this.width, this.height);
  }
}

// Іменування класу (поганий приклад)
// class rectangle {
//     constructor(width, height) {
//     this.width = width;
//     this.height = height;
//     }
// }
  
// Кодування на основі тестування
function testRectangle() {
  console.assert(calculateArea(5, 10) === 50,
   "Area function test failed. Make changes to the code logic.");
  console.assert(calculatePerimeter(5, 10) === 30, "Perimeter function test failed");
  
  const testRectangle = new Rectangle(5, 10);
  console.assert(testRectangle.calculateArea() === 50, "Rectangle class area test failed");
  console.assert(testRectangle.calculatePerimeter() === 30, "Rectangle class perimeter test failed");
  
  console.log("All tests passed.");
}
  
let rectangleWidth = 5;
let rectangleHeight = 10;
  
if (validateDimensions(rectangleWidth, rectangleHeight)) {
  const myRectangle = new Rectangle(rectangleWidth, rectangleHeight);
  
  console.log(`${RECTANGLE_NAME} Area:`, myRectangle.calculateArea());
  console.log(`${RECTANGLE_NAME} Perimeter:`, myRectangle.calculatePerimeter());
} else {
  console.log("Invalid dimensions.");
}
  
testRectangle();

/*
// Гарний приклад використання відступів, вирівнювання, фігурних дужок 
function validateDimensions(width, height) {
  return width > 0 && height > 0;
}

function calculatePerimeter(width, height) {
  return 2 * (width + height);
}

// Поганий приклад використання відступів, фігурних дужок, відсутність вирівнювання
function calculateArea(width, height){
  return width*height;
  }function calculatePerimeter(width,height){return 2*(width+height);}

// Відсутнє дотримання правила довжини (80–120 символів). 
console.assert(calculateArea(5, 10) === 50, "Area function test failed. Make changes to the code logic.");

// Розбиття рядка для покращення читабельності.
console.assert(calculateArea(5, 10) === 50,
 "Area function test failed. Make changes to the code logic.");
*/