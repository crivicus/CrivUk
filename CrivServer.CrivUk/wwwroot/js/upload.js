/// <reference path="models.js" />

var File = (function () {
    function File(toUpload) {
        this.toUpload = toUpload;
    }
    return File;
}());

var myApp = angular.module('myApp', []);
myApp.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

myApp.service('fileService', ['$http', function ($http) {
    this.createFile = function (file) {
        var fd = new FormData();
        fd.append('toUpload', file.toUpload);

        return $http.post('/api/file/upload', fd, {
            transformRequest: angular.identity,
            headers: {
                'Content-Type': undefined
            }
        });
    };
}]);

myApp.controller('myCtrl', ['$scope', 'fileService', function ($scope, fileService) {
    $scope.createFile = function () {
        $scope.showUploadStatus = false;
        $scope.showUploadedData = false;

        var file = new File($scope.toUpload);

        fileService.createFile(file).then(function (response) { // success
            if (response.status == 200) {
                $scope.uploadStatus = "File uploaded sucessfully.";
                $scope.uploadedData = response.data;
                $scope.showUploadStatus = true;
                $scope.showUploadedData = true;
                $scope.errors = [];
            }
        },
            function (response) { // failure
                $scope.uploadStatus = "File upload failed with status code: " + response.status;
                $scope.showUploadStatus = true;
                $scope.showUploadedData = false;
                $scope.errors = [];
                $scope.errors = parseErrors(response);
            });
    };
}]);

function parseErrors(response) {
    var errors = [];
    for (var key in response.data) {
        for (var i = 0; i < response.data[key].length; i++) {
            errors.push(key + ': ' + response.data[key][i]);
        }
    }
    return errors;
}