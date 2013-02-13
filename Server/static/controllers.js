'use strict';

/* Controllers */

function StatusCtrl($scope, $http, $timeout) {
    var statusCount = 8;

    var getFromApi = function() {
        $http.get('chemstationstatus/last/' + statusCount).success(function(data) {
            $scope.statuses = data;
            $scope.currentStatus = data[0];
        });
    };
    getFromApi();

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
    $scope.statusForward = function(currentStatusId) {
        $http.get('chemstationstatus/' + (currentStatusId + 1)).success(function(data) {
            if (!isEmpty(data)) {
                $scope.statuses.pop();
                $scope.statuses.unshift(data);
                $scope.currentStatus = $scope.statuses[0];
            }
        });

    };

    // Update the viewModel by dropping the first item and adding a new one on the bottom.
    $scope.statusBackward = function(currentStatusId) {
        $http.get('chemstationstatus/' + (currentStatusId - statusCount)).success(function(data) {
            if (!isEmpty(data)) {
                $scope.statuses.shift();
                $scope.statuses.push(data);
                $scope.currentStatus = $scope.statuses[0];
            }
        });
    };
}

/* Helpers */

function isEmpty(object) {
    for(var i in object) {
        return false;
    } return true;
}