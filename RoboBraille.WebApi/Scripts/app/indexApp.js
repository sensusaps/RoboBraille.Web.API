var app2 = angular.module('testApp', ['emguo.poller']);

app2.controller('statusController', function ($scope, poller) {
    var url = 'http://2.109.50.18:5150';
    var statusResultApiPath = url + '/api/RoboBrailleJob';
    $scope.statusCode = 2;
    $scope.flag = 'Checking';
    $scope.pollStatus = function () {
        var myPoller = poller.get(statusResultApiPath + '/GetJobStatus', {
            action: 'get',
            delay: 5000,
            argumentsArray: [
                {
                    params: { jobId: '4513e973-0ea4-e511-82b6-606c66415a40' }
                }
            ],
            smart: true
        });

        myPoller.promise.then(
            null,
            null,
            function (data) {
                $scope.statusCode = data.data;
                if ($scope.flag == 'Checking')
                    $scope.flag = 'Looking........';
                else $scope.flag = 'Checking';
            }
        );
    };
});