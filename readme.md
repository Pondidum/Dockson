# Dockson, a Continuous Delivery Measurement Service

**Insipired by [Measuring Continuous Delivery: The what, why, and how of measuring Continuous Delivery](https://www.goodreads.com/book/show/35508935-measuring-continuous-delivery)**

* notifications
  ```json
  {
    "timestamp": "<datetime>",
    "source": "", // github, teamcity, etc.
    "type": "commit|build|deploy|incident",
    "name": "", // "my service", "master"
    "version": "", // "1.0.1"
    "status": "success|fail",
    "tags": [] // branch, team name, service group, etc.
  }
  ```
* measurements
  * deployment stability
    * `d = deployments, f = failures, t = time`
    * failure rate
      * percentage of deployments which cause prod error
      * `dfr = percent( f in t / d in t )`
    * failure recovery time
      * time between start and end of production failures
      * `dfrt.median = median( ( f.end - f.start) for f in t )`
      * `dfrt.stddev = stddev( ( f.end - f.start) for f in t )`
    * graphs:
      * axes: `x = time, y1 = dfr, y2 = dfrt`
      * lines: `dfr, dfrt.median, dfrt.stddev`
  * deployment throughput
    * `dn = deploy time, n = build finish time, d = prod deployments, t = time`
    * lead time
      * time between creating package and it's deployment
      * `dlt.median = median( (d^n - b^n ) for d in t )`
      * `dlt.stddev = stddev( (d^n - b^n ) for d in t )`
    * frequency / interval
      * time between production deployments
      * `di.median = median( (d^n - d^n-1 ) for d in t )`
      * `di.stddev = stddev( (d^n - d^n-1 ) for d in t )`
    * graphs:
      * axes: `x = time, y = days`
      * lines: `dlt.median, dlt.stddev, di.median, di.stddev`
  * build stability
    * `b = builds, f = failures, t = time`
    * failure rate
      * percent of builds which fail
      * `bfr = percent( f in t / b in t )`
    * failure recovery time
      * time between start and end of failures
      * `bfrt.median = median( ( b^n+1 - b^n ) for f in t )`
      * `bfrt.stddev = stddev( ( b^n+1 - b^n ) for f in t )`
    * graphs
      * axes: `x = time, y1 = bfr, y2 = bfrt`
      * lines: `bfr, bfrt.median, bfrt.stddev`
  * build throughput
    * `m^n = commit time, b = builds, b^n = build finish time, t = time`
    * build lead time
      * time between master commit and build artifact
      * `blt.median = median( ( b^n- m^n ) for b in t )`
      * `blt.stddev = stddev( ( b^n- m^n ) for b in t )`
    * build interval
      * time between artifact publishes
      * `bi.median = median( (b^n - n^n-1 ) for b in t )`
      * `bi.stddev = stddev( (b^n - n^n-1 ) for b in t )`
    * graphs
      * axes: `x = time, y = hours`
      * lines: `blt.median, blt.stddev, bi.median, bi.stddev`
  * mainline throughput
    * `m^n = master commit time, b^n = branch commit time, m = master commits, t = time`
    * mainline lead time
      * time between commit to branch and merge to master
      * `mlt.median = median( ( m^n - b^n ) for m in t )`
      * `mlt.stddev = stddev( ( m^n - b^n ) for m in t )`
    * mainline interval
      * time between master commits
      * `mi.median = median( ( m^n - m^n-1) for m in t )`
      * `mi.stddev = stddev( ( m^n - m^n-1) for m in t )`
    * graphs
      * axes: `x = time, y = hours`
      * lines: `mlt.median, mlt.stddev, mi.median, mi.stddev`
* base architecture
  * ![overview](architecture.png)
* eventing architecture
  * masterCommit projection:
    * emits `MasterCommitEvent` event when a masterCommit matches with a branchCommit
    * `[ ...commitNotification... ] => MasterCommit(sourceCommit, masterCommit)`
  * masterLeadTime projection
    * populates `MasterLeadTimeView` from `MasterCommitEvent`s
  * masterInterval projection
    * populates `MasterIntervalView` with intervals between `MasterCommitEvent`s
