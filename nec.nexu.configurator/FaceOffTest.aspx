<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FaceOffTest.aspx.cs" Inherits="nec.nexu.configurator.FaceOffTest" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>New Era By You Face Off</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="/assets/css/reset.css" rel="stylesheet" type="text/css" />
    <link href="/assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/kendo/styles/kendo.common-bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/kendo/styles/kendo.flat.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
    <link href="/assets/css/nexu.css" rel="stylesheet" type="text/css" />
    
    <script src="/assets/js/jquery-2.1.0.min.js"></script>
    <script src="/assets/kendo/js/kendo.all.min.js"></script>
    <script src="/assets/js/customBreakpoints.js"></script>
    <script src="/assets/nexu/nexu.common.js"></script>



</head>
<body>
    <div class="container">

        <div class="row">
            <div class="nx-faceoff-container row hidden">

                <div class="nx-faceoff-circles  hidden-xs">
                    <div class="nx-circle-container nx-left col-xs-6">
                        <div class="nx-circle"
                            data-bind="attr: { data-total: FinishedProductAVoteCount, data-percentage: PercentageA, data-bg:FinishedProductABackgroundColor }">&nbsp;</div>
                    </div>

                    <div class="nx-circle-container nx-right col-xs-6">
                        <div class="nx-circle"
                            data-bind="attr: { data-total: FinishedProductBVoteCount, data-percentage: PercentageB, data-bg:FinishedProductBBackgroundColor}">&nbsp;</div>
                    </div>
                </div>

                <div class="nx-faceoff-headline row">
                    <h2>
                        <a href="\\#" data-bind="attr: { href: PreviousFaceOffLink}, visible: NavLinks" class="hidden-xs">
                            <span class="icon icon-40b-left hidden-xs"></span></a>
                        <a href="\\#" data-bind="attr: { href: PreviousFaceOffLink}, visible: NavLinks" class="hidden-sm hidden-md hidden-lg">
                            <span class="icon icon-20b-left"></span></a>
                        <span  data-bind="text: Title"></span>
                        <a href="\\#"  data-bind="attr: { href: NextFaceOffLink }, visible: NavLinks" class="hidden-xs">
                            <span class="icon icon-40b-right"></span></a>
                        <a href="\\#"  data-bind="attr: { href: NextFaceOffLink }, visible: NavLinks" class="hidden-sm hidden-md hidden-lg">
                            <span class="icon icon-20b-right visible-sm-*"></span></a>
                    </h2>
                    <h4 data-bind="text: Description"></h4>
                </div>

                <div class="nx-faceoff-caps row">

                    <div class="nx-faceoff-panel nx-left col-xs-12 col-sm-5">
                        <div class="nx-faceoff-fill nx-left hidden-sm hidden-md hidden-lg" data-bind="attr: { data-bg:FinishedProductABackgroundColor }">&nbsp;</div>
                        <div>
                            <!-- img node should be placed via javascript so that we can add the dynamic size attributes -->
                            <input class="nx-hidden-image" type="hidden" data-bind="value: FinishedProductA.DetailPath" />
                            <div class="nx-faceoff-percentage nx-left"><span data-bind="text: PercentageA"></span>%</div>
                        </div>

                        <div  class="nx-faceoff-users">
                            <div class="nx-left">
                                <div class="col-xs-12  col-sm-12">
                                    <div class="nx-user-image">
                                        <img data-bind="attr: { src: FinishedProductA.CreatorProfileImagePath }" />
                                    </div>
                                    <span class="nx-user-image" data-bind="text: FinishedProductA.CreatorName"></span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="nx-mid col-sm-2 hidden-xs hidden-sm">
                        <span data-bind="style: {opacity: HasCurrentUserVotedForA}" class="nx-check nx-left icon icon-40b-check transparent"></span>
                        <a href="\\#" class="nx-vote nx-left" data-bind="attr: { data-id: FinishedProductA.Id, data-disabled:HasCurrentUserVoted }">
                            <span class="icon icon-40b-left"></span>
                        </a>
                        <b>OR</b>
                        <a href="\\#" class="nx-vote nx-right" data-bind="attr: { data-id: FinishedProductB.Id, data-disabled:HasCurrentUserVoted }">
                            <span class="icon icon-40b-right"></span>
                        </a>
                        <span data-bind="style: {opacity: HasCurrentUserVotedForB}" class="nx-check nx-right icon icon-40b-check transparent"></span>
                    </div>

                    <div class="nx-mid nx-mobile col-sm-2 hidden-md hidden-lg">
                        <!-- This class binding may seem backwards, but since we are hiding the check mark that is NOT voted for,
                            we actually want to bind to the porperty of the opposite-->
                        <span data-bind="class:{ transparent: HasCurrentUserVotedForB}" class="nx-check icon icon-40b-check transparent"></span>
                        <a href="\\#" class="nx-vote nx-left" data-bind="attr: { dataId: FinishedProductA.Id }"><span class="icon icon-40b-left"></span></a>
                        <b>VS</b>
                        <a href="\\#" class="nx-vote nx-right" data-bind="attr: { dataId: FinishedProductB.Id }"><span class="icon icon-40b-right"></span></a>
                        <span data-bind="class:{ transparent: HasCurrentUserVotedForA}" class="nx-check icon icon-40b-check transparent"></span>
                    </div>
                    

                    <div class="nx-faceoff-panel col-xs-12 nx-right col-sm-5">
                            <div class="nx-faceoff-fill nx-right hidden-sm hidden-md hidden-lg" data-bind="attr: { data-bg:FinishedProductBBackgroundColor }">&nbsp;</div>
                            <div>
                                <!-- img node should be placed via javascript so that we can add the dynamic size attributes -->
                                <input class="nx-hidden-image" type="hidden" data-bind="value: FinishedProductB.DetailPath" />
                                <div class="nx-faceoff-percentage nx-right"><span data-bind="text: PercentageB"></span>%</div>
                            </div>

                            <div  class="nx-faceoff-users">
                                <div class="nx-right">
                                    <div class="col-xs-12 col-sm-12  pull-right">
                                        <span class="nx-user-image" data-bind="text: FinishedProductB.CreatorName"></span>
                                        <div class="nx-user-image">
                                            <img data-bind="attr: { src: FinishedProductB.CreatorProfileImagePath }" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                    </div>

                </div>

                <div class="nx-total col-xs-12">
                    <span class="nx-faceoff-total" data-bind="text: TotalVotes">0</span> VOTES CAST
                </div>

            </div>
        </div>

    </div>
    <script type="text/javascript">






    </script>

</body>
</html>
