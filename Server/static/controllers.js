'use strict';

/* Controllers */

function StatusCtrl($scope, $http, $timeout) {
    var statusCount = 6;

    var getFromApi = function() {
        $http.get('chemstationstatus/last/' + statusCount).success(function(data) {
            $scope.statuses = data;
            $scope.currentStatus = data[0];
        });
    };
    getFromApi();

    $http.get('/chemstationstatus/statistics/500').success(function(stats) {
          $scope.data = stats;
    });

    // Refresh the viewModel every so often via AJAX.
    $scope.onTimeout = function(){
        getFromApi();
        mytimeout = $timeout($scope.onTimeout,1000 * 60 * 10);
    }
    var mytimeout = $timeout($scope.onTimeout,1000 * 60 * 10);

    $scope.viewStatus = function(status) {
        $scope.currentStatus = status;
    };

    // Update the viewModel by dropping the last item and adding a new one on top.
    $scope.statusForward = function() {
        $http.get('chemstationstatus/' + ($scope.statuses[0].Id + 1)).success(function(data) {
            if (!isEmpty(data)) {
                $scope.statuses.pop();
                $scope.statuses.unshift(data);
                $scope.currentStatus = $scope.statuses[0];
            }
        });

    };

    // Update the viewModel by dropping the first item and adding a new one on the bottom.
    $scope.statusBackward = function() {
        $http.get('chemstationstatus/' + ($scope.statuses[0].Id - statusCount)).success(function(data) {
            if (!isEmpty(data)) {
                $scope.statuses.shift();
                $scope.statuses.push(data);
                $scope.currentStatus = $scope.statuses[0];
            }
        });
    };
}

/* Modules */

// This directive encapsulates the d3.js status pie chart widget.
angular.module('chemstation', []).directive('statusChart', function($http) {

    var w = 350,                        //width
        h = 350,                            //height
        r = w/2,                            //radius
        color = d3.scale.ordinal()
            .range(["#98abc5", "#8a89a6", "#7b6888", "#6b486b", "#a05d56", "#d0743c", "#ff8c00"]);
    return {
        restrict: 'E',
        scope: {
         val: '='
        },
        link: function (scope, element, attrs) {
        //d3 code adapted from http://bl.ocks.org/enjalot/1203641
            var data;
            var rootElement = element[0];
            var getAngle = function (d) {
                return (180 / Math.PI * (d.startAngle + d.endAngle) / 2 - 90)
            };

            scope.$watch('val', function (newVal, oldVal) {
                 // if 'val' is undefined, exit
                if (!newVal) {
                return;
                }
                // Not interested in displaying statuses that are very uncommon for statistics purposes.
                data = newVal.data.filter(function(item) { return item.value >= 2;});

                var vis = d3.select(rootElement)
                    .append("svg:svg")
                    .data([data])
                    .attr("viewBox", "0 0 " + w + " " + h )
                    .attr("preserveAspectRatio", "xMidYMid meet")          // make the pie chart responsive to device width.
                    .append("svg:g")                                       //make a group to hold our pie chart
                    .attr("transform", "translate(" + r + "," + r + ")")   //move the center of the pie chart from

                var arc = d3.svg.arc()                                     //create <path> elements for us using arc data
                    .outerRadius(r);

                var pie = d3.layout.pie()           //this will create arc data for us given a list of values
                    .value(function(d) { return d.value; });    //we must tell it out to access the value of each element in our data array

                var arcs = vis.selectAll("g.slice")
                    .data(pie)
                    .enter()
                    .append("svg:g")
                    .attr("class", "slice");

                arcs.append("svg:path")
                    .attr("fill", function(d, i) { return color(i); } )
                    .attr("d", arc);             //this creates the SVG path using the associated data (pie) with the arc drawing function

                    arcs.append("svg:text")
                        .attr("transform", function(d) {
                            d.innerRadius = 0;
                            d.outerRadius = r;
                            return "translate(" + arc.centroid(d) + ") " +
                                "rotate(" + getAngle(d) + ")";
                        })
                        .attr("dy", 5)
                        .style("text-anchor", "start")
                        .text(function(d, i) { return data[i].label.substring(0,11); });
            });
        }
    }
});

/* Helpers */

function isEmpty(object) {
    for(var i in object) {
        return false;
    } return true;
}