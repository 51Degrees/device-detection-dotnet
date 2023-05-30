#!/bin/sh

# Constants
PASSES=20000
PROJECT=..
HOST=localhost:5000
CAL=calibrate
PRO=process
PERF=./ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.sh

echo "current dir: "
pwd

$PERF -n $PASSES -s "$PROJECT/output/performance_tests"  -c $CAL -p $PRO -h $HOST