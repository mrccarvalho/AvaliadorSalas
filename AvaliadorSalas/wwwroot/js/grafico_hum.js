

function drawChartHum() {

    $.ajax(
        {
            url: '/Home/ReadingDayHum',
            dataType: "json",
            data: {},
            type: "GET",
            success: function (jsonData) {

                // create blank data table
                var data = new google.visualization.DataTable();

                // parse json
                jsonData.forEach(function (jsonRow, indexRow) {
                    // add columns
                    if (indexRow === 0) {
                        for (var column in jsonRow) {
                            if (column === 'timeOnlyString') {
                                data.addColumn('string', 'HORAS');
                            } else if (column === 'humidString') {
                                data.addColumn('number', '%');

                            }
                        }
                    }

                    // add row
                    var dataRow = [];
                    for (var column in jsonRow) {
                        if (column === 'timeOnlyString') {
                            dataRow.push(jsonRow[column]);
                        } else if (column === 'humidString') {
                            // convert string to number
                            dataRow.push(parseFloat(jsonRow[column]));
                        }
                    }
                    if (dataRow.length > 0) {
                        data.addRow(dataRow);
                    }
                });
                var options = {
                    chart: { title: '' },
                    colors: ['red']
                };
                var chart = new google.charts.Line(document.getElementById('chart_div_hum'));
                chart.draw(data, options);
            }
        });

    return false;
}

