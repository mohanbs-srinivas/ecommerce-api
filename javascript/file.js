async function add(a, b) {
  return a + b;
}

function multiply(a, b) {
  return a * b;
}

async function divide(a, b) {
  if (b === 0) {
    throw new Error("Division by zero");
  }
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      resolve(a * b); 
    }, 1000);
  });
}

function subtract(a, b) {
  return a - b;
}

console.log("Addition result:", await add(5, 3)); 

divide(10, 2)
  .then((result) => console.log("Division result:", result))
  .catch((error) => console.error("Error:", error.message));
