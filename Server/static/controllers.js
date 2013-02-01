'use strict';

/* Controllers */

function StatusCtrl($scope, $http, $timeout) {
    var getFromApi = function() {
        $http.get('chemstationstatus/last/5').success(function(data) {
        $scope.statuses = data;
        $scope.currentStatus = data[0];
        });
    };
    getFromApi();

    $scope.onTimeout = function(){
        getFromApi();
        mytimeout = $timeout($scope.onTimeout,1000 * 60 * 5);
    }
    var mytimeout = $timeout($scope.onTimeout,1000 * 60 * 5);

    $scope.viewStatus = function(status) {
        $scope.currentStatus = status;
    };
}