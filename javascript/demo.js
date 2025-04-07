//Write a JavaScript function that reads the content of a CSV file, converts each row into a JSON object.
// and stores the JSON objects as string values in an array.

function csvToJson(csv) {
    const lines = csv.split('\n');
    const result = [];
    const headers = lines[0].split(',');

    for (let i = 1; i < lines.length; i++) {
        const obj = {};
        const currentline = lines[i].split(',');

        for (let j = 0; j < headers.length; j++) {
            obj[headers[j]] = currentline[j];
        }

        result.push(JSON.stringify(obj));
    }

    return result;
}