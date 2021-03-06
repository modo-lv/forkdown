'use strict'

class ForkdownItems {
  /**
   * @param  {ForkdownProfileSet} profileSet
   */
  constructor({profileSet}) {
    this.profileSet = profileSet
    this.index = {}
  }

  /**
   *
   * @return {ForkdownProfile}
   */
  profile = () => this.profileSet.activeProfile

  /**
   *
   */
  init = async () => {
    // Singles
    this.index = $("#fd--singles-index").data("index")
    if (this.index.length < 1)
      throw new Error("No singles index!")

    for (const key in this.index) {
      this.index[key].forEach(id => {
        $(document.getElementById(id)).data("fd--single-id", key)
      })
    }

    // CHECKBOXES
    $(".fd--checkbox > input").each((i, c) => {
      let box = $(c)
      let id = box.prop("id")
      if (!id) {
        console.log(c)
        throw new Error("Checkbox without an ID")
      }

      box.on("change", async () => {
        let checked = this.mark(id)
        if (box.data("fd--single-id"))
          this.markSingles(id, checked);
        return this.profileSet.saveProfile()
      })
      box.prop("checked", this.profile().isChecked(id))
    })
  }

  mark = (id, on = null) => {
    let box = document.getElementById(id)
    if (box) {
      if (on != null) {
        console.debug("Setting " + box.id + " to checked: ", on)
        $(box).prop("checked", on);
      }
      on = $(box).prop("checked")
    }
    console.debug("Saving " + id + " as checked: ", on)
    this.profile().toggleCheck(id, on);
    return on
  }

  markSingles = (id, on) => {
    let box = document.getElementById(id)
    let indexId = $(box).data("fd--single-id")
    for (let a = 0; a < this.index[indexId].length; a++) {
      let boxId = this.index[indexId][a];
      if (boxId === box.id)
        continue
      this.mark(boxId, on)
    }
  }
}